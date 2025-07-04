using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KnowledgeTestVisualization.EF;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeTestVisualization.Model
{
    public class GroupPerformanceIndicators
    {
        public GroupPerformanceIndicators(Group group, Subject subject, int lecturerId)
        {
            _group = group;
            _subject = subject;
            _lecturerId = lecturerId;

            CalculateAcademicPerformance();
            CalculateAverageGrade();
            CalculateLearningMasteryIndex();
            CalculateQualitativePerformance();
        }

        public GroupPerformanceIndicators()
        {
        }

        private Group _group;
        private Subject _subject;
        private int _lecturerId;

        public double AcademicPerformance { get; private set; }               // > 3.0
        public double KnowledgeQuality { get; private set; }        // > 4.0
        public double AverageGrade { get; private set; }                      // Средний балл (0–5)
        public double LearningMasteryIndex { get; private set; }              // СОУ (0–100)
        public double QualitativePerformance { get; private set; } // Качество успеваемости (КУ)

        private void CalculateAcademicPerformance()
        {
            var dbContext = new KnowledgeTestDbContext();
            AcademicPerformance = Math.Round(
                GetGroupPerformanceAsync(dbContext, _lecturerId, _subject.Id, _group.Id, 3).Result * 100d, 2);

            KnowledgeQuality = Math.Round(
                GetGroupPerformanceAsync(dbContext, _lecturerId, _subject.Id, _group.Id, 4).Result * 100d, 2);
        }

        private void CalculateAverageGrade()
        {
            var dbContext = new KnowledgeTestDbContext();
            AverageGrade = Math.Round(
                GetAverageGradeAsync(dbContext, _lecturerId, _subject.Id, _group.Id).Result, 2);
        }

        private void CalculateLearningMasteryIndex()
        {
            var dbContext = new KnowledgeTestDbContext();
            LearningMasteryIndex = Math.Round(
                GetLearningMasteryIndexAsync(dbContext, _lecturerId, _subject.Id, _group.Id).Result, 2);
        }
        private void CalculateQualitativePerformance()
        {
            var dbContext = new KnowledgeTestDbContext();
            QualitativePerformance = Math.Round(
                GetQualitativePerformanceAsync(dbContext, _lecturerId, _subject.Id, _group.Id).Result, 2);
        }

        private async Task<double> GetGroupPerformanceAsync(
            KnowledgeTestDbContext db,
            int lecturerId,
            int subjectId,
            int groupId,
            double minPerformancePoint,
            CancellationToken ct = default)
        {
            var studentsInGroup = db.Students
                                    .Where(s => s.GroupId == groupId)
                                    .Select(s => s.Id);

            var avgMarksQuery =
                from v in db.VwJournalDetaileds
                join t in db.Tests on v.TestId equals t.Id
                where v.LecturerId == lecturerId
                      && t.SubjectId == subjectId
                      && studentsInGroup.Contains(v.StudentId)
                group v by v.StudentId into g
                select g.Average(x => x.Mark);

            var successful = await avgMarksQuery
                .Where(avg => avg > minPerformancePoint)
                .CountAsync(ct);

            var total = await studentsInGroup.CountAsync(ct);

            return total == 0 ? 0d : (double)successful / total;
        }

        private async Task<double> GetAverageGradeAsync(
            KnowledgeTestDbContext db,
            int lecturerId,
            int subjectId,
            int groupId,
            CancellationToken ct = default)
        {
            var studentIds = await db.Students
                .Where(s => s.GroupId == groupId)
                .Select(s => s.Id)
                .ToListAsync(ct);

            if (!studentIds.Any())
                return 0;

            var marks = await (
                from v in db.VwJournalDetaileds
                join t in db.Tests on v.TestId equals t.Id
                where v.LecturerId == lecturerId && t.SubjectId == subjectId
                select new { v.StudentId, v.Mark }
            ).ToListAsync(ct);

            var relevantMarks = marks
                .Where(x => studentIds.Contains(x.StudentId))
                .Select(x => x.Mark);

            return relevantMarks.Any()
                ? relevantMarks.Average()
                : 0;
        }


        private async Task<double> GetLearningMasteryIndexAsync(
            KnowledgeTestDbContext db,
            int lecturerId,
            int subjectId,
            int groupId,
            CancellationToken ct = default)
        {
            var studentIds = db.Students
                               .Where(s => s.GroupId == groupId)
                               .Select(s => s.Id);

            var avgPerStudent =
                from v in db.VwJournalDetaileds
                join t in db.Tests on v.TestId equals t.Id
                where v.LecturerId == lecturerId
                      && t.SubjectId == subjectId
                      && studentIds.Contains(v.StudentId)
                group v by v.StudentId into g
                select new { StudentId = g.Key, Avg = g.Average(x => x.Mark) };

            var avgDict = await avgPerStudent.ToDictionaryAsync(x => x.StudentId, x => x.Avg, ct);
            var totalStudents = await studentIds.CountAsync(ct);
            if (totalStudents == 0) return 0;

            int n5 = 0, n4 = 0, n3 = 0, n2 = 0, nNA = 0;

            foreach (var id in studentIds)
            {
                if (!avgDict.TryGetValue(id, out double avg))
                {
                    nNA++;
                    continue;
                }

                int mark = (int)Math.Round(avg, MidpointRounding.AwayFromZero);
                switch (mark)
                {
                    case 5: n5++; break;
                    case 4: n4++; break;
                    case 3: n3++; break;
                    case 2: n2++; break;
                    default: nNA++; break;
                }
            }

            double sou = (n5 * 100 + n4 * 64 + n3 * 36 + n2 * 16 + nNA * 7) / (double)totalStudents;
            return sou;
        }

        private async Task<double> GetQualitativePerformanceAsync(
            KnowledgeTestDbContext db,
            int lecturerId,
            int subjectId,
            int groupId,
            CancellationToken ct = default)
        {
            var studentIds = await db.Students
                .Where(s => s.GroupId == groupId)
                .Select(s => s.Id)
                .ToListAsync(ct);

            if (!studentIds.Any())
                return 0;

            var marks = await (
                from v in db.VwJournalDetaileds
                join t in db.Tests on v.TestId equals t.Id
                where v.LecturerId == lecturerId
                      && t.SubjectId == subjectId
                      && studentIds.Contains(v.StudentId)
                select v.Mark
            ).ToListAsync(ct);

            if (marks.Count == 0)
                return 0;

            int goodMarks = marks.Count(m => m == 4 || m == 5);
            return (double)goodMarks / marks.Count * 100;
        }
    }
}
