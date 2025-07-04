using KnowledgeTestVisualization.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeTestVisualization.Model
{
    public class TestQuestionResult
    {
        public int QuestionNumber { get; set; }
        public int PointsScored { get; set; }
        public int MaxPoints { get; set; }
        public double SuccessRate { get; set; } 
    }

    public class StudentTestResult
    {
        public int StudentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string LastNameAndInitialsName
        {
            get
            {
                if (string.IsNullOrEmpty(LastName))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(FirstName))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(Patronymic))
                {
                    return $"{LastName} {FirstName[0]}.";
                }
                return $"{LastName} {FirstName[0]}. {Patronymic[0]}.";
            }
        }
        public List<TestQuestionResult> QuestionResults { get; set; }
    }

    public class TestDetailsService
    {
        private static StudentTestResult GetTestDetailsForStudent(int studentId, int testId)
        {
            var context = new KnowledgeTestDbContext();

            // Получаем последнюю попытку студента по этому тесту
            var latestAttempt = context.VwJournalDetaileds
                .Where(j => j.StudentId == studentId && j.TestId == testId)
                .OrderByDescending(j => j.CreateTime)
                .FirstOrDefault();

            if (latestAttempt == null)
                return null;

            // Получаем детальные результаты по вопросам
            var questionResults = from jq in context.JournalQuestions
                                  join q in context.Questions on jq.QuestionId equals q.Id
                                  where jq.JournalId == latestAttempt.JournalId
                                  orderby q.QuestionNumber
                                  select new TestQuestionResult
                                  {
                                      QuestionNumber = q.QuestionNumber,
                                      PointsScored = jq.PointsScored,
                                      MaxPoints = q.MaxPoints,
                                      SuccessRate = (jq.PointsScored / (double)q.MaxPoints)
                                  };

            // Получаем информацию о студенте
            var student = context.Students.FirstOrDefault(s => s.Id == studentId);

            return new StudentTestResult
            {
                StudentId = studentId,
                LastName = student?.LastName,
                FirstName = student?.FirstName,
                Patronymic = student?.Patronymic,
                QuestionResults = questionResults.ToList()
            };
        }
        public static StudentTestResult[] GetTestDetails(int groupId, int testId)
        {
            var context = new KnowledgeTestDbContext();

            List<StudentTestResult> result = new List<StudentTestResult>();

            var studentIds = context.Students.Where(s=>s.GroupId == groupId).Select(s=>s.Id);
            foreach (var studentId in studentIds) {
                var testResult = GetTestDetailsForStudent(studentId, testId);
                if (testResult is null) {
                    continue;
                }
                result.Add(testResult);
            }

            return result.OrderBy(s => s.LastNameAndInitialsName).ToArray();
        }
    }
}
