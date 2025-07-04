using KnowledgeTestVisualization.EF;
 
namespace KnowledgeTestVisualization.Model
{
    public class TestStudentsIndicators
    {
        public string TestName
        {
            get
            {
                return _test.Name;
            }
        }
        public int Mark { get; private set; }
        private DateTime _creationDate;
        public string CreationDate
        {
            get { return _creationDate.ToShortDateString() + " " + _creationDate.ToShortTimeString(); }
        }
        public int AttempsCount { get; private set; }
        private Test _test;
        public double TotalPoints { get; private set; }
        public double MaxTotalPoints { get; private set; }
        public double PerformanceRate
        {
            get
            {
                return Math.Round(TotalPoints / MaxTotalPoints * 100, 2);
            }
        }
        public TestStudentsIndicators(Test test, int mark, double totalPoints, double maxTotalPoints, int attempsCount, DateTime creationDate)
        {
            _test = test;
            Mark = mark;
            AttempsCount = attempsCount;
            _creationDate = creationDate;
            TotalPoints = totalPoints;
            MaxTotalPoints = maxTotalPoints;
        }
        public TestStudentsIndicators(int testId, int mark, double totalPoints, double maxTotalPoints, int attempsCount, DateTime creationDate)
        {
            var dbContext = new KnowledgeTestDbContext();
            _test = dbContext.Tests.Where(t=>t.Id==testId).First();
            if (_test == null)
            {
                throw new ArgumentException("Тест не может быть null");
            }
            Mark = mark;
            AttempsCount = attempsCount;
            _creationDate = creationDate;
            TotalPoints = totalPoints;
            MaxTotalPoints = maxTotalPoints;
        }
        public override string ToString()
        {
            if (AttempsCount == 0)
            {
                return TestName;
            }
            return $"{TestName}. Попытка от {CreationDate}";
        }
    }
}