using KnowledgeTestVisualization.Model;
using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public static async Task<List<Group>> GetGroupsAsync()
    {
        return await Task.Run(async () =>
        {
            var dbContext = new KnowledgeTestDbContext();
            var session = Session.GetSession();
            var currLector = (from l in dbContext.Lecturers where l.Id == session.Account.LecturerId select l).First();

            var allStudentsByLector = from j in dbContext.Journals
                                      where j.Lecturer == currLector
                                      select j.Student;

            var allGroupNamesByStudents = allStudentsByLector.Select(st => st.Group)
                .OrderBy(g => g.Name)
                .Select(g => g.Name)
                .Distinct();

            var list = allGroupNamesByStudents.ToList();

            List<Group> groups = new List<Group>();
            foreach (var groupName in list)
            {
                var group = (from g in dbContext.Groups where g.Name == groupName select g).First();
                groups.Add(group);
            }
            return groups.OrderBy(g=>g.Name).ToList();
        });
    }
    public override string ToString()
    {
        return Name;
    }
}
