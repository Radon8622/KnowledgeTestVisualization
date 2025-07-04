using KnowledgeTestVisualization.Model;
using KnowledgeTestVisualization.EF;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeTestVisualization.ViewModel
{
    class StudentVisualizationPgViewModel
    {
        public async Task<List<Group>> GetGroupsNameAsync()
        {
            return await Group.GetGroupsAsync();
        }
        public async Task<List<Student>> GetStudentsNameByGroupAsync(string groupName)
        {
            return await Student.GetStudentsNameByGroupAsync(groupName);
        }
        public async Task<List<Subject>> GetSubjectsByStudentAsync(Student student, int lectorId)
        {
            return await Task.Run(async () =>
            {
                var dbContext = new KnowledgeTestDbContext();
                var currLector = (from l in dbContext.Lecturers where l.Id == lectorId select l).First();

                var allStudentLearningSubjects = from j in dbContext.Journals
                                                 where j.StudentId == student.Id && j.LecturerId == currLector.Id
                                                 select j.Subject;

                var list = allStudentLearningSubjects.Distinct().ToList();
                return list;
            });
        }


        public async Task<StudentInfo> GetMainStudentInformationAsync(Student student, Subject subject, int lecturerd)
        {
            StudentInfo mainStudentInformation = new StudentInfo();
            return await Task.Run(() => {
                return mainStudentInformation = new StudentInfo(student, subject, lecturerd);
            });
        }

        public async Task<TestStudentsIndicators[]> GetTestsInfoAsync(Student student, Subject subject, int lecturerd)
        {
            return await Task.Run(async () =>
            {
                using var dbContext = new KnowledgeTestDbContext();
                var journalEntries = dbContext.VwJournalDetaileds
                    .Where(s => s.StudentId == student.Id)
                    .ToList();

                // Фильтруем записи по выбранному предмету
                var testIdsForSubject = dbContext.Tests
                    .Where(t => t.SubjectId == subject.Id && t.LecturerId == lecturerd)
                    .Select(t => t.Id)
                    .ToHashSet();

                var filteredEntries = journalEntries
                    .Where(entry => testIdsForSubject.Contains(entry.TestId))
                    .OrderBy(entry => entry.CreateTime)
                    .ToList();

                var filteredTestIds = filteredEntries.Select(w => w.TestId).ToList();

                var tests = dbContext.Tests
                    .Join(
                        filteredTestIds,
                        test => test.Id,
                        testId => testId,
                        (test, _) => test
                    )
                    .ToList();

                List<TestStudentsIndicators> studentTests = new List<TestStudentsIndicators>();
                for ( var i = 0; i < filteredEntries.Count; i++)
                {
                    studentTests.Add(new TestStudentsIndicators(tests.First(t=>t.Id == filteredEntries[i].TestId), filteredEntries[i].Mark, (double)filteredEntries[i].TotalPoints, (double)filteredEntries[i].TotalMaxPoints, 0, filteredEntries[i].CreateTime));
                }
                return studentTests.OrderBy(t => Convert.ToDateTime(t.CreationDate)).ToArray();
            });
        }

        public async Task<AttemptInfo[]> GetTestsAttemptsInfoAsync(Student student, Subject subject, int lecturerd)
        {
            if (student == null || subject == null)
                return Array.Empty<AttemptInfo>();

            using var dbContext = new KnowledgeTestDbContext();

            try
            {
                // 1. Сначала получаем все тесты для предмета
                var subjectTests = await dbContext.Tests
                    .Where(t => t.SubjectId == subject.Id && t.LecturerId == lecturerd)
                    .Select(t => t.Name)
                    .ToListAsync();

                if (!subjectTests.Any())
                    return Array.Empty<AttemptInfo>();

                // 2. Получаем попытки студента по этим тестам
                var attempts = await dbContext.VwJournalAttempts
                    .Where(a => a.StudentId == student.Id && subjectTests.Contains(a.TestName))
                    .OrderBy(a => a.CreateTime)
                    .ToListAsync();

                if (!attempts.Any())
                    return Array.Empty<AttemptInfo>();

                // 3. Группируем на клиенте
                var groupedAttempts = attempts
                    .GroupBy(a => a.TestName)
                    .ToList();

                // 4. Получаем полные данные тестов
                var testNames = groupedAttempts.Select(g => g.Key).ToList();
                var testsDict = await dbContext.Tests
                    .Where(t => testNames.Contains(t.Name))
                    .ToDictionaryAsync(t => t.Name);

                // 5. Формируем результат
                var result = new List<AttemptInfo>();

                foreach (var group in groupedAttempts)
                {
                    if (!testsDict.TryGetValue(group.Key, out var test))
                        continue;

                    var attemptInfos = group.Select(a => new AttemptInfo(
                        testName: a.TestName,
                        mark: a.Mark,
                        creationDate: a.CreateTime,
                        totalPoints: a.TotalPoints ?? 0,
                        maxTotalPoints: a.TotalMaxPoints ?? 1
                    )).ToArray();

                    result.AddRange(attemptInfos);
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                return Array.Empty<AttemptInfo>();
            }
        }

        public async Task<(Question, int PointsMax, int PointsTotal)[]> GetAnswersInfoAsync(AttemptInfo attempt)
        {
            if (attempt == null)
                return Array.Empty<(Question, int PointsMax, int PointsTotal)>();

            using var dbContext = new KnowledgeTestDbContext();

            try
            {
                var journalId = await dbContext.Journals.
                    Where(w => w.Test.Name == attempt.TestName && w.CreateTime == attempt.CreationDate).
                    Select(w => w.Id).
                    FirstAsync();

                var attemptAnswers = await dbContext.JournalQuestions.
                    Where(w=> w.JournalId == journalId).
                    OrderBy(w=>w.QuestionId).ToListAsync();


                var questionsInfos = attemptAnswers.Select(a => new { a.QuestionId, a.PointsScored }).ToList();

                var result = new List<(Question, int PointsMax, int PointsTotal)>();

                for (int i = 0; i < attemptAnswers.Count; i++)
                {
                    var question = await dbContext.Questions.Where(q=>q.Id == questionsInfos[i].QuestionId).FirstAsync();
                    result.Add((question, question.MaxPoints, questionsInfos[i].PointsScored));
                }

                return result.ToArray();
            }
            catch
            {
                return Array.Empty<(Question, int PointsMax, int PointsTotal)>();
            }
        }
    }
}
