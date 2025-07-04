using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class JournalQuestion
{
    public int Id { get; set; }

    public int JournalId { get; set; }

    public int QuestionId { get; set; }

    public int PointsScored { get; set; }

    public virtual Journal Journal { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}
