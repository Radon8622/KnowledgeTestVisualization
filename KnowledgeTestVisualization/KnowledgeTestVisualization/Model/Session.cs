using System;
using System.Collections.Generic;
using System.Text;
using KnowledgeTestVisualization.EF;

namespace KnowledgeTestVisualization.Model
{
    public class Session
    {
        private static Session _actualSession;
        public UserAccount Account {  get; private set; }
        private Session(UserAccount userAccount)
        {
            Account = userAccount;
        }
        public static void CreateSession(UserAccount userAccount)
        {
            if (!(_actualSession is null)) { 
                DestroySession();
            }
            _actualSession = new Session(userAccount);
        }
        public static Session GetSession()
        {
            if (_actualSession == null) 
            {
                throw new Exception("Нет активной сессии");
            }
            return _actualSession;
        }
        public static void DestroySession()
        {
            if (_actualSession == null)
            {
                throw new Exception("Нет активной сессии");
            }
            _actualSession = null;
        }
    }
}
