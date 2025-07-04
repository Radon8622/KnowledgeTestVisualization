using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class Question
{
    public int Id { get; set; }

    public int TestId { get; set; }

    public int QuestionNumber { get; set; }

    public int MaxPoints { get; set; }

    public virtual ICollection<JournalQuestion> JournalQuestions { get; set; } = new List<JournalQuestion>();

    public virtual Test Test { get; set; } = null!;
}
