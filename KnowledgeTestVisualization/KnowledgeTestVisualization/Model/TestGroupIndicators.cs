using KnowledgeTestVisualization.EF;
 
namespace KnowledgeTestVisualization.Model
{
    public class TestGroupIndicators
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        private double? _averageMark;
        public double? AverageMark
        {
            get => Math.Round(_averageMark??0,2);
            set => _averageMark = value;
        }
        private double? _averageScore;
        public double? AverageScore
        {
            get => Math.Round(_averageScore ?? 0, 2);
            set => _averageScore = value;
        }
        public int? MaxPossibleScore { get; set; }
        public double? AveragePerformanceRate
        {
            get => Math.Round((_averageScore*100d/ MaxPossibleScore) ?? 0, 2);
        }
        public override string ToString()
        {
            return TestName;
        }

        public static TestGroupIndicators[] GetTestStatistics(int lecturerId, int subjectId, int groupId)
        {
            var dbContext = new KnowledgeTestDbContext();
            var query = from journal in dbContext.VwJournalDetaileds
                        join student in dbContext.Students on journal.StudentId equals student.Id
                        where student.GroupId == groupId
                        join test in dbContext.Tests on journal.TestId equals test.Id
                        where test.LecturerId == lecturerId && test.SubjectId == subjectId
                        group new { journal, test } by new { test.Id, test.Name } into g
                        select new TestGroupIndicators
                        {
                            TestId = g.Key.Id,
                            TestName = g.Key.Name,
                            AverageMark = g.Average(x => x.journal.Mark),
                            AverageScore = g.Average(x => x.journal.TotalPoints),
                            MaxPossibleScore = g.Max(x => x.journal.TotalMaxPoints)
                        };

            return query.ToArray();
        }
    }
}