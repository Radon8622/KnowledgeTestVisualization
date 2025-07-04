using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnowledgeTestVisualization.EF;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace KnowledgeTestVisualization.Model
{
    public class StudentInfo
    {
        public StudentInfo(Student student, Subject subject, int lecturerId)
        {
            _student = student;
            _subject = subject;
            _lecturerId = lecturerId;
            SetTestsInfo();
            CalulateAvgMark();
            CalulateAvgPointsRatio();
            CalulateDinamicValue();
        }
        public StudentInfo()
        {
        }
        private Student _student;
        private Subject _subject;
        private int _lecturerId;
        public double AvgMark { get; private set; }
        public double AvgPointsRatio { get; private set; }
        public double? DinamicValue { get; private set; }
        private List<TestStudentsIndicators> _testsInfo;
        public List<TestStudentsIndicators> TestsInfo {
            get
            {
                return _testsInfo.OrderBy(t => t.CreationDate).ToList();
            }
            set
            {
                _testsInfo = value;
            }
        }

        private void SetTestsInfo()
        {
            _testsInfo = new List<TestStudentsIndicators>();
            var tests = new KnowledgeTestDbContext().Tests.Where(t => t.SubjectId == _subject.Id);
            foreach (var test in tests) {
                try
                {
                    var select = new KnowledgeTestDbContext().VwJournalDetaileds.Where(w =>
                    w.TestId == test.Id &&
                    w.StudentId == _student.Id &&
                    w.LecturerId == _lecturerId
                    ).Select(w => new { w.Mark, w.CreateTime, w.AttemptCount , w.TotalPoints, w.TotalMaxPoints}).First();
                    _testsInfo.Add(new TestStudentsIndicators(test, select.Mark, select.TotalPoints ?? 0, select.TotalMaxPoints ?? 0, select.AttemptCount ?? 0, select.CreateTime));
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
        private void CalulateAvgMark()
        {
            var tests = new KnowledgeTestDbContext().Tests.Where(t => t.SubjectId == _subject.Id && t.LecturerId == _lecturerId);

            List<int> marks = new List<int>();
            foreach (var test in tests)
            {
                var selects = new KnowledgeTestDbContext().VwJournalDetaileds.Where(w =>
                    w.TestId == test.Id &&
                    w.StudentId == _student.Id
                    ).Select(w => w.Mark).ToList();
                marks.AddRange(selects);
            }
            AvgMark = marks.Average();
        }
        private void CalulateAvgPointsRatio()
        {
            var tests = new KnowledgeTestDbContext().Tests.Where(t => t.SubjectId == _subject.Id && t.LecturerId == _lecturerId);

            List<double?> points = new List<double?>();
            foreach (var test in tests)
            {
                var selects = new KnowledgeTestDbContext().VwJournalDetaileds.Where(w =>
                    w.TestId == test.Id &&
                    w.StudentId == _student.Id
                    ).Select(w => ((double)w.TotalPoints / w.TotalMaxPoints)).ToList();
                points.AddRange(selects);
            }
            foreach (var mark in points)
            {
                if (mark == null)
                {
                    continue;
                }
                AvgPointsRatio += (double)mark;
            }
            AvgPointsRatio /= points.Count;

            AvgPointsRatio = Math.Round(AvgPointsRatio * 100, 2);
        }
        private void CalulateDinamicValue()
        {
            List<int> tests = new List<int>();
            var testsListSelects = new KnowledgeTestDbContext().Tests.Where(t => t.SubjectId == _subject.Id && t.LecturerId == _lecturerId).Select(t => t.Id).ToList();
            var selects = new KnowledgeTestDbContext().VwJournalDetaileds.Where(w =>
                w.StudentId == _student.Id && testsListSelects.Contains(w.TestId)).OrderBy(w=>w.CreateTime).Select(w => new { w.TotalPoints, w.TotalMaxPoints, w.CreateTime }).ToList();
            if (selects.Count < 2) 
            {
                DinamicValue = null;
                return;
            }
            var current = selects.Last().TotalPoints * 1d / selects.Last().TotalMaxPoints;
            var previous = selects[selects.Count-2].TotalPoints * 1d / selects[selects.Count - 2].TotalMaxPoints;
            DinamicValue = Math.Round((double)((current - previous) * 100d / previous), 2);
        }
    }
}
