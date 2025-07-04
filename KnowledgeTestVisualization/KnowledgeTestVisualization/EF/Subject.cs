using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public virtual ICollection<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
