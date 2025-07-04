using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class QuestionBuffer
{
    public int Id { get; set; }

    public Guid SessionId { get; set; }

    public int TestId { get; set; }

    public int QuestionNumber { get; set; }

    public int MaxPoints { get; set; }

    public DateTime? CreatedAt { get; set; }
}
