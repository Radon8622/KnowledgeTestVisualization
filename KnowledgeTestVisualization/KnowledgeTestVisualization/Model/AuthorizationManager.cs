using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.Windows;
using KnowledgeTestVisualization.ViewModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KnowledgeTestVisualization.EF;

namespace KnowledgeTestVisualization.Model
{
    enum AuthorizeStatus
    {
        Fine,
        Fail,
        Error
    }
    class AuthorizationManager
    {
        public static async Task<AuthorizeStatus> AuthorizeAsync(string login, string password)
        {
            return await Task.Run(async () =>
            {
                var hashedPassword = ComputeSHA256Hash(password);
                bool haveConnection;
                var db = KnowledgeTestDbContext.GetDBContext(out haveConnection);

                if (!haveConnection)
                    return AuthorizeStatus.Error;

                var account = await db.UserAccounts
                    .FirstOrDefaultAsync(ac => Equals(ac.Username,login) && Equals(ac.PasswordHash, hashedPassword));

                if (account == null)
                    return AuthorizeStatus.Fail;

                if (account.Username != login)
                    return AuthorizeStatus.Fail;

                Session.CreateSession(account);
                return AuthorizeStatus.Fine;
            });
        }

        private static byte[] ComputeSHA256Hash(string message)
        {
            StringBuilder builder = new StringBuilder();
            byte[] bytes;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                bytes = sha256Hash.ComputeHash(Encoding.Unicode.GetBytes(message));
            }
            return bytes;
        }
    }
}
