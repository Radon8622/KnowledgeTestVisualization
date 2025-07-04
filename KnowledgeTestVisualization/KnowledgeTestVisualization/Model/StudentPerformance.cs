using KnowledgeTestVisualization.EF;

public class StudentPerformance
{
    public int StudentId { get; set; }
    public string LastNameAndInitialsName { get
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
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Patronymic { get; set; }
    public double AverageMark { get; set; }
    public double AverageSuccessRate { get; set; }
    public double AverageAttemptCount { get; set; }
}
public class StudentPerformanceService
{
    public static StudentPerformance[] GetStudentPerformanceByGroup(int lecturerId, int subjectId, int groupId)
    {
        var _context = new KnowledgeTestDbContext();
        var query = from journal in _context.VwJournalDetaileds
                    join student in _context.Students on journal.StudentId equals student.Id
                    where student.GroupId == groupId
                    join test in _context.Tests on journal.TestId equals test.Id
                    where test.LecturerId == lecturerId && test.SubjectId == subjectId
                    group new { journal, student } by new
                    {
                        student.Id,
                        student.LastName,
                        student.FirstName,
                        student.Patronymic
                    } into g
                    select new StudentPerformance
                    {
                        StudentId = g.Key.Id,
                        LastName = g.Key.LastName,
                        FirstName = g.Key.FirstName,
                        Patronymic = g.Key.Patronymic,
                        AverageMark = Math.Round(g.Average(x => x.journal.Mark),2),
                        AverageSuccessRate = Math.Round(g.Average(x => (double)x.journal.TotalPoints / (double)x.journal.TotalMaxPoints*100d),2),
                        AverageAttemptCount = Math.Round(g.Average(x=> x.journal.AttemptCount ?? 0), 2)
                    };

        return query.ToArray();
    }
}
