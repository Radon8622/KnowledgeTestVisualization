using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace KnowledgeTestVisualization.EF
{
    public partial class Student
    {
        public override string ToString() => $"{LastName} {FirstName[0]}. {Patronymic[0]}.";
    }
}
