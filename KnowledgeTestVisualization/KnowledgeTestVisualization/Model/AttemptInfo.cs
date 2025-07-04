using System;
using KnowledgeTestVisualization.EF;

namespace KnowledgeTestVisualization.Model
{
    public class AttemptInfo
    {
        private Test _test;
        private string _testName;
        public string TestName
        {
            get 
            {
                if (_test is null)
                {
                    return _testName;
                }
                return _test.Name;
            }
        }
        public int Mark { get; }
        public DateTime CreationDate { get; }
        public double TotalPoints { get; }
        public double MaxTotalPoints { get; }
        public double PerformanceRate => Math.Round(TotalPoints / MaxTotalPoints * 100, 2);

        public AttemptInfo(Test test, int mark, DateTime creationDate, double totalPoints, double maxTotalPoints)
        {
            _test = test;
            Mark = mark;
            CreationDate = creationDate;
            TotalPoints = totalPoints;
            MaxTotalPoints = maxTotalPoints;
        }
        public AttemptInfo(string testName, int mark, DateTime creationDate, double totalPoints, double maxTotalPoints)
        {
            _testName = testName;
            Mark = mark;
            CreationDate = creationDate;
            TotalPoints = totalPoints;
            MaxTotalPoints = maxTotalPoints;
        }
        public override string ToString() 
        { 
            return TestName + ". " + CreationDate;
        }
    }
}