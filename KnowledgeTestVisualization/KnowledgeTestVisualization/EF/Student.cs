using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class Student
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public int GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
    public static async Task<List<Student>> GetStudentsNameByGroupAsync(string groupName)
    {
        return await Task.Run(async () =>
        {
            var dbContext = new KnowledgeTestDbContext();
            EF.Group _group;
            try
            {
                _group = (from g in dbContext.Groups where g.Name == groupName select g).First();
            }
            catch (Exception)
            {
                return null;
            }

            var allStudentsByGroup = from st in dbContext.Students
                                     where st.Group == _group
                                     select st;

            var students = allStudentsByGroup
                .Distinct().OrderBy(st => st.LastName).ToList();

            return students;
        });
    }
}
