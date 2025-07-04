using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class Lecturer
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public virtual ICollection<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
}
