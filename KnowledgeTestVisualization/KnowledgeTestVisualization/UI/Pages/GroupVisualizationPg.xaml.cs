using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using KnowledgeTestVisualization.ViewModel;
using LiveChartsCore.SkiaSharpView.WPF;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using LiveChartsCore.ConditionalDraw;
using System.Windows.Data;
using LiveChartsCore.Measure;
using System.IO;
using KnowledgeTestVisualization.Model;
using System.Globalization;
using LiveChartsCore.Defaults;
using System.Linq;

namespace KnowledgeTestVisualization.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для GroupVisualizationPg.xaml
    /// </summary>
    public partial class GroupVisualizationPg : Page
    {
        private GroupVisualizationPgViewModel _modelView;
        private int? _lecturerId;
        public GroupVisualizationPg()
        {
            InitializeComponent();
            _modelView = new GroupVisualizationPgViewModel();
            _lecturerId = Session.GetSession().Account.LecturerId;
        }
        public async void DrawTestsAvgMarksСhart()
        {
            ChartsStudentExp.Visibility = Visibility.Visible;
            var testsInfo = await _modelView.GetTestsInfoAsync(GroupsCbx.SelectedItem as EF.Group, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? 0);

            // Создаём серию данных для графика
            var columnSeries = new ColumnSeries<double>
            {
                Values = testsInfo.Select(t => (double)t.AverageMark).ToArray(),
                Stroke = null,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                IgnoresBarPosition = true,
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    int index = (int)chartPoint.Coordinate.SecondaryValue;
                    return $"Средний балл: {chartPoint.Coordinate.PrimaryValue}";
                }
            };

            // Окрашиваем колонки в зависимости от значения
            columnSeries.OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                var value = point.Coordinate.PrimaryValue;

                if (value >= 4.5)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(60, 179, 113)); // Зеленый
                }
                else if (value >= 3.5)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(140, 203, 95)); // Светло-зеленый
                }
                else if (value >= 2.5)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(225, 204, 79)); // Желтый
                }
                else
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(205, 74, 76)); // Красный
                }
            });

            // Настраиваем ось Y
            var yAxis = new Axis
            {
                Name = "Средняя отметка"
            };

            // Настраиваем ось X для отображения дат
            var xAxis = new Axis
            {
                Labels = testsInfo.Select(t => t.TestName).ToArray(),
                LabelsRotation = 0,
                LabelsPaint = new SolidColorPaint(SKColors.Black)
            };

            // Применяем настройки к графику
            TestsAvgMarksChart.Series = new ISeries[] { columnSeries, };
            TestsAvgMarksChart.YAxes = new[] { yAxis };
            TestsAvgMarksChart.XAxes = new[] { xAxis };
            TestsAvgMarksChart.UpdateLayout();
        }
        public async void DrawTestsAvgPerformanceСhart()
        {
            ChartsStudentExp.Visibility = Visibility.Visible;
            var testsInfo = await _modelView.GetTestsInfoAsync(GroupsCbx.SelectedItem as EF.Group, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? 0);

            // Создаём серию данных для графика
            var columnSeries = new ColumnSeries<double>
            {
                Values = testsInfo.Select(t => (double)t.AveragePerformanceRate).ToArray(),
                Stroke = null,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                IgnoresBarPosition = true,
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    int index = (int)chartPoint.Coordinate.SecondaryValue;
                    return $"Средняя результативность: {chartPoint.Coordinate.PrimaryValue}%";
                }
            };

            // Окрашиваем колонки в зависимости от значения
            columnSeries.OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                var value = point.Coordinate.PrimaryValue;

                if (value >= 80)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(60, 179, 113)); // Зеленый
                }
                else if (value >= 65)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(140, 203, 95)); // Светло-зеленый
                }
                else if (value >= 45)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(225, 204, 79)); // Желтый
                }
                else
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(205, 74, 76)); // Красный
                }
            });

            // Настраиваем ось Y
            var yAxis = new Axis
            {
                Name = "Средняя результативность (%)"
            };

            // Настраиваем ось X для отображения дат
            var xAxis = new Axis
            {
                Labels = testsInfo.Select(t => t.TestName).ToArray(),
                LabelsRotation = 0,
                LabelsPaint = new SolidColorPaint(SKColors.Black)
            };

            // Применяем настройки к графику
            TestsAvgPerformanceChart.Series = new ISeries[] { columnSeries, };
            TestsAvgPerformanceChart.YAxes = new[] { yAxis };
            TestsAvgPerformanceChart.XAxes = new[] { xAxis };
            TestsAvgPerformanceChart.UpdateLayout();
        }
        public async void DrawStudentsAvgMarksСhart()
        {
            ChartsStudentExp.Visibility = Visibility.Visible;
            var studentsInfo = await _modelView.GetStudentsInformationAsync(GroupsCbx.SelectedItem as EF.Group, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? 0);

            // Создаём серию данных для графика
            var columnSeries = new ColumnSeries<double>
            {
                Values = studentsInfo.Select(t => (double)t.AverageMark).ToArray(),
                Stroke = null,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                IgnoresBarPosition = true,
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    int index = (int)chartPoint.Coordinate.SecondaryValue;
                    return $"Средний балл: {chartPoint.Coordinate.PrimaryValue}";
                }
            };

            // Окрашиваем колонки в зависимости от значения
            columnSeries.OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                var value = point.Coordinate.PrimaryValue;

                if (value >= 4.5)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(60, 179, 113)); // Зеленый
                }
                else if (value >= 3.5)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(140, 203, 95)); // Светло-зеленый
                }
                else if (value >= 2.5)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(225, 204, 79)); // Желтый
                }
                else
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(205, 74, 76)); // Красный
                }
            });

            // Настраиваем ось Y
            var yAxis = new Axis
            {
                Name = "Средний балл"
            };

            var xAxis = new Axis
            {
                Labels = studentsInfo.Select(t => t.LastNameAndInitialsName).ToArray(),
                LabelsRotation = -35,
                LabelsPaint = new SolidColorPaint(SKColors.Black)
            };

            // Применяем настройки к графику
            StudentsAvgMarksChart.Series = new ISeries[] { columnSeries, };
            StudentsAvgMarksChart.YAxes = new[] { yAxis };
            StudentsAvgMarksChart.XAxes = new[] { xAxis };
            StudentsAvgMarksChart.UpdateLayout();
        }
        public async void DrawTestsAvgPerformanceDetailedСhart()
        {
            ChartsStudentExp.Visibility = Visibility.Visible;

            var testInfo = (TestNameCbx.SelectedItem as TestGroupIndicators);
            var studentTestResults = await _modelView.GetStudentsTestResultsAsync(GroupsCbx.SelectedItem as EF.Group, testInfo.TestId);
            var questionsCount = _modelView.GetTestQuestionsCount(testInfo.TestId);

            // Создаём серию данных для графика

            List<WeightedPoint> heatmapValues = new List<WeightedPoint>();
            for (int i = 0; i < studentTestResults.Length; i++)
            {
                for (int j = 0; j < studentTestResults[i].QuestionResults.Count; j++)
                {
                    heatmapValues.Add(new(j, i, studentTestResults[studentTestResults.Length-i-1].QuestionResults[j].SuccessRate));
                }
            }

            List<string> questionsNumbers = new List<string>();
            for (int i = 1; i <= questionsCount; i++)
            {
                questionsNumbers.Add(i.ToString());
            }

            var columnSeries = new HeatSeries<WeightedPoint>
            {
                HeatMap = [
                    new SKColor(97, 81, 164).AsLvcColor(),
                    new SKColor(210, 207, 222).AsLvcColor(),
                ],
                Values = heatmapValues.ToArray(),
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    int j = (int)chartPoint.Coordinate.SecondaryValue;
                    int i = studentTestResults.Length - (int)chartPoint.Coordinate.PrimaryValue - 1;
                    var testInfo = studentTestResults[i].QuestionResults[j];
                    return $"Набрано {Math.Round(testInfo.SuccessRate*100d,2)}% баллов ({testInfo.PointsScored}/{testInfo.MaxPoints})";
                }
            };

            // Настраиваем ось Y
            var yAxis = new Axis
            {
                Labels = studentTestResults.Select(r=>r.LastNameAndInitialsName).Reverse().ToArray(),
                Name = "Студент"
            };

            var xAxis = new Axis
            {
                Labels = questionsNumbers.ToArray(),
                Name = "Номер вопроса"
            };

            // Применяем настройки к графику
            TestsAvgPerformanceDetailedChart.Series = new ISeries[] { columnSeries, };
            TestsAvgPerformanceDetailedChart.YAxes = new[] { yAxis };
            TestsAvgPerformanceDetailedChart.XAxes = new[] { xAxis };
            TestsAvgPerformanceDetailedChart.UpdateLayout();
        }
        private async void WriteTestsAvgMarksTable()
        {
            TestsAvgMarksDtg.Visibility = Visibility.Visible;

            var mainStudentInformation = await _modelView.GetTestsInfoAsync(GroupsCbx.SelectedItem as EF.Group, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? 0);

            TestsAvgMarksDtg.ItemsSource = mainStudentInformation;
            TestsAvgMarksDtg.AutoGenerateColumns = false;
            TestsAvgMarksDtg.Columns.Clear();
            TestsAvgMarksDtg.Columns.Add(new DataGridTextColumn { Header = "Тест", Binding = new Binding("TestName") });
            TestsAvgMarksDtg.Columns.Add(new DataGridTextColumn { Header = "Средняя отметка", Binding = new Binding("AverageMark") });
        }
        private async void WriteTestsAvgPerformanceTable()
        {
            TestsAvgPerformanceDtg.Visibility = Visibility.Visible;

            var mainStudentInformation = await _modelView.GetTestsInfoAsync(GroupsCbx.SelectedItem as EF.Group, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? 0);

            TestsAvgPerformanceDtg.ItemsSource = mainStudentInformation;
            TestsAvgPerformanceDtg.AutoGenerateColumns = false;
            TestsAvgPerformanceDtg.Columns.Clear();
            TestsAvgPerformanceDtg.Columns.Add(new DataGridTextColumn { Header = "Тест", Binding = new Binding("TestName") });
            TestsAvgPerformanceDtg.Columns.Add(new DataGridTextColumn { Header = "Средняя результативность (%)", Binding = new Binding("AveragePerformanceRate") });
            TestsAvgPerformanceDtg.Columns.Add(new DataGridTextColumn { Header = "Средний балл", Binding = new Binding("AverageScore") });
            TestsAvgPerformanceDtg.Columns.Add(new DataGridTextColumn { Header = "Максимальное число баллов в тесте", Binding = new Binding("MaxPossibleScore ") });
        }
        private async void WriteStudentsAvgMarksTable()
        {
            StudentsAvgMarksDtg.Visibility = Visibility.Visible;

            var studentsInfo = await _modelView.GetStudentsInformationAsync(GroupsCbx.SelectedItem as EF.Group, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? 0);

            StudentsAvgMarksDtg.ItemsSource = studentsInfo;
            StudentsAvgMarksDtg.AutoGenerateColumns = false;
            StudentsAvgMarksDtg.Columns.Clear();
            StudentsAvgMarksDtg.Columns.Add(new DataGridTextColumn { Header = "Студент", Binding = new Binding("LastNameAndInitialsName") });
            StudentsAvgMarksDtg.Columns.Add(new DataGridTextColumn { Header = "Средняя отметка", Binding = new Binding("AverageMark") });
            StudentsAvgMarksDtg.Columns.Add(new DataGridTextColumn { Header = "Средняя результативность (%)", Binding = new Binding("AverageSuccessRate") });
            StudentsAvgMarksDtg.Columns.Add(new DataGridTextColumn { Header = "Среднее число попыток", Binding = new Binding("AverageAttemptCount") });
        }
        private async void WriteTestsAvgPerformanceDetailedTable()
        {
            AnswersGrd.Visibility = Visibility.Visible;

            var testGroup = TestNameCbx.SelectedItem as TestGroupIndicators;

            AvgTestMarkTbx.Text = testGroup.AverageMark.ToString();
            AvgTestPerformanceTbx.Text = $"{testGroup.AveragePerformanceRate}%";
        }

        //Обработчики событий

        private async void GroupPage_Loaded(object sender, RoutedEventArgs e)
        {
            GroupsCbx.ItemsSource = await _modelView.GetGroupsAsync();
        }
        private async void GroupNameCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsCbx.SelectedValue is null)
            {
                DisableComboBox(SubjectCbx);
                return;
            }
            try
            {
                SubjectCbx.IsEnabled = true;
                var source = await _modelView.GetSubjectsByGroupAsync(GroupsCbx.SelectedValue.ToString(), _lecturerId??-1);
                if (source.Count == 0)
                {
                    DisableComboBox(SubjectCbx);
                    return;
                }
                SubjectCbx.ItemsSource = source;
            }
            catch (Exception)
            {
                DisableComboBox(SubjectCbx);
            }
        }

        private async void SubjectCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubjectCbx.SelectedValue is null)
            {
                GeneralStudentInfoExp.Visibility = Visibility.Collapsed;
                ChartsStudentExp.Visibility = Visibility.Collapsed;

                return;
            }
            try
            {
                TestNameCbx.IsEnabled = true;
                var source = await _modelView.GetTestsInfoAsync(GroupsCbx.SelectedItem as EF.Group, SubjectCbx.SelectedItem as EF.Subject, _lecturerId??-1);
                if (source.Count() == 0)
                {
                    DisableComboBox(TestNameCbx);
                    return;
                }
                TestNameCbx.ItemsSource = source;
                TestNameCbx.SelectedIndex = 0;
            }
            catch (Exception)
            {
                DisableComboBox(TestNameCbx);
                return;
            }

            WriteMainGroupInformation();

            DrawTestsAvgMarksСhart();
            WriteTestsAvgMarksTable();

            DrawTestsAvgPerformanceСhart();
            WriteTestsAvgPerformanceTable();

            DrawStudentsAvgMarksСhart();
            WriteStudentsAvgMarksTable();
        }
        private void TestNameCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(TestNameCbx.SelectedItem is TestGroupIndicators))
            {
                TestsAvgPerformanceDetailedChart.Visibility = Visibility.Collapsed;
                return;
            }
            TestsAvgPerformanceDetailedChart.Visibility = Visibility.Visible;

            DrawTestsAvgPerformanceDetailedСhart();
            WriteTestsAvgPerformanceDetailedTable();
        }
        private void AcademicPerformanceRefInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Отношение числа учащихся, чья средняя отметка выше чем 3 балла, к общему числу учащихся в группе.", "Краткая справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void KnowledgeQualityRefInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Отношение числа учащихся, чья средняя отметка выше чем 4 балла, к общему числу учащихся в группе.", "Краткая справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void QualitativePerformanceRefInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Отношение числа учащихся, чья средняя отметка выше чем 4 балла, к числу учащихся с средней оценкой 5 баллов.", "Краткая справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void AverageGradeRefInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Среднее арифметическое полученных студентами оценок.", "Краткая справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void LearningMasteryIndexRefInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Процент усвоенного материала.", "Краткая справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Управление UI
        private void DisableComboBox(ComboBox comboBox)
        {
            comboBox.ItemsSource = null;
            comboBox.Items.Clear();
            comboBox.IsEnabled = false;
        }
        private async void WriteMainGroupInformation()
        {
            GeneralStudentInfoExp.Visibility = Visibility.Visible;
            var mainStudentInformation = await _modelView.GetMainGroupInformationAsync(GroupsCbx.SelectedItem as EF.Group, SubjectCbx.SelectedItem as EF.Subject, Session.GetSession().Account.LecturerId??0);
            AcademicPerformanceTbx.Text = mainStudentInformation.AcademicPerformance.ToString() + "%";

            if (mainStudentInformation.AcademicPerformance < 50)
            {
                AcademicPerformanceTbx.Foreground = Brushes.Red;
            }
            else if (mainStudentInformation.AcademicPerformance > 75)
            {
                AcademicPerformanceTbx.Foreground = Brushes.Green;
            }
            else
            {
                AcademicPerformanceTbx.Foreground = Brushes.Black;
            }

            KnowledgeQualityTbx.Text = mainStudentInformation.KnowledgeQuality.ToString() + "%";

            if (mainStudentInformation.KnowledgeQuality < 33)
            {
                KnowledgeQualityTbx.Foreground = Brushes.Red;
            }
            else if (mainStudentInformation.KnowledgeQuality > 50)
            {
                KnowledgeQualityTbx.Foreground = Brushes.Green;
            }
            else
            {
                KnowledgeQualityTbx.Foreground = Brushes.Black;
            }

            QualitativePerformanceTbx.Text = mainStudentInformation.QualitativePerformance.ToString() + "%";

            if (mainStudentInformation.QualitativePerformance < 50)
            {
                QualitativePerformanceTbx.Foreground = Brushes.Red;
            }
            else if (mainStudentInformation.QualitativePerformance > 75)
            {
                QualitativePerformanceTbx.Foreground = Brushes.Green;
            }
            else
            {
                QualitativePerformanceTbx.Foreground = Brushes.Black;
            }

            AverageGradeTbx.Text = mainStudentInformation.AverageGrade.ToString();

            if (mainStudentInformation.AverageGrade < 3.5)
            {
                AverageGradeTbx.Foreground = Brushes.Red;
            }
            else if (mainStudentInformation.AverageGrade > 4)
            {
                AverageGradeTbx.Foreground = Brushes.Green;
            }
            else
            {
                AverageGradeTbx.Foreground = Brushes.Black;
            }

            LearningMasteryIndexTbx.Text = mainStudentInformation.LearningMasteryIndex.ToString() + "%";

            if (mainStudentInformation.LearningMasteryIndex < 60)
            {
                LearningMasteryIndexTbx.Foreground = Brushes.Red;
            }
            else if (mainStudentInformation.LearningMasteryIndex > 75)
            {
                LearningMasteryIndexTbx.Foreground = Brushes.Green;
            }
            else
            {
                LearningMasteryIndexTbx.Foreground = Brushes.Black;
            }
        }
    }
}
