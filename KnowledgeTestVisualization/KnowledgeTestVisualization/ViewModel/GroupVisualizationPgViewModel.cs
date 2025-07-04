using KnowledgeTestVisualization.Model;
using KnowledgeTestVisualization.EF;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeTestVisualization.ViewModel
{
    class GroupVisualizationPgViewModel
    {
        public async Task<List<Group>> GetGroupsAsync()
        {
            return await Group.GetGroupsAsync();
        }
        public async Task<List<Subject>> GetSubjectsByGroupAsync(string groupName, int lecturerId)
        {
            return await Task.Run(async () =>
            {
                var dbContext = new KnowledgeTestDbContext();
                var currLector = (from l in dbContext.Lecturers where l.Id == lecturerId select l).First();

                var students = await Student.GetStudentsNameByGroupAsync(groupName);

                List<Subject> result = new List<Subject>();

                foreach (var student in students)
                {
                    var allStudentLearningSubjects = from j in dbContext.Journals
                                                     where j.StudentId == student.Id && j.LecturerId == currLector.Id
                                                     select j.Subject;

                    result.AddRange(allStudentLearningSubjects);
                }
                return result.Distinct().ToList();
            });
        }
        public async Task<GroupPerformanceIndicators> GetMainGroupInformationAsync(Group group, Subject subject, int lecturerId)
        {
            return await Task.Run(() => {
                return new GroupPerformanceIndicators(group, subject, lecturerId);
            });
        }
        public async Task<TestGroupIndicators[]> GetTestsInfoAsync(Group group, Subject subject, int lecturerId)
        {
            return await Task.Run(async () =>
            {
                return TestGroupIndicators.GetTestStatistics(lecturerId, subject.Id, group.Id);
            });
        }
        public async Task<StudentPerformance[]> GetStudentsInformationAsync(Group group, Subject subject, int lecturerd)
        {
            return await Task.Run(() => {
                return StudentPerformanceService.GetStudentPerformanceByGroup(lecturerd, subject.Id, group.Id).OrderBy(s => s.LastNameAndInitialsName).ToArray();
            });
        }
        public async Task<StudentTestResult[]> GetStudentsTestResultsAsync(Group group, int testId)
        {
            return await Task.Run(() => {
                return TestDetailsService.GetTestDetails(group.Id, testId);
            });
        }
        public int GetTestQuestionsCount(int testId)
        {
            using var dbContext = new KnowledgeTestDbContext();
            return dbContext.Questions.Count(q => q.TestId == testId);
        }
    }
}
