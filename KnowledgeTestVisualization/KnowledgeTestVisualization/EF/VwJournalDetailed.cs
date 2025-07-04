using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class VwJournalDetailed
{
    public int JournalId { get; set; }

    public DateTime CreateTime { get; set; }

    public int StudentId { get; set; }

    public int LecturerId { get; set; }

    public int TestId { get; set; }

    public int? TotalMaxPoints { get; set; }

    public int? TotalPoints { get; set; }

    public double? PercentScore { get; set; }

    public int Mark { get; set; }

    public int? AttemptCount { get; set; }

    public int? CorrectAnswers { get; set; }

    public int? WrongAnswers { get; set; }

    public int? ZeroAnswers { get; set; }
}
