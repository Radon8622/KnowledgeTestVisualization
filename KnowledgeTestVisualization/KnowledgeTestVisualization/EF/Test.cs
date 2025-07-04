using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class Test
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int LecturerId { get; set; }

    public int SubjectId { get; set; }

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public virtual Lecturer Lecturer { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual Subject Subject { get; set; } = null!;
}
