using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class Journal
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int SubjectId { get; set; }

    public int LecturerId { get; set; }

    public int TestId { get; set; }

    public DateTime CreateTime { get; set; }

    public virtual ICollection<JournalQuestion> JournalQuestions { get; set; } = new List<JournalQuestion>();

    public virtual Lecturer Lecturer { get; set; } = null!;

    public virtual LecturerSubject LecturerSubject { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
