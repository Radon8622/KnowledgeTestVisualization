using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class LecturerSubject
{
    public int Id { get; set; }

    public int LecturerId { get; set; }

    public int SubjectId { get; set; }

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public virtual Lecturer Lecturer { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
