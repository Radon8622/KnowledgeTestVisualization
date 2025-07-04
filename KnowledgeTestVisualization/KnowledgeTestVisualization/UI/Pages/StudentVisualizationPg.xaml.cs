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

namespace KnowledgeTestVisualization.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для StudentVisualizationPg.xaml
    /// </summary>
    public partial class StudentVisualizationPg : Page
    {
        private StudentVisualizationPgViewModel _modelView;
        private int? _lecturerId;
        public StudentVisualizationPg()
        {
            InitializeComponent();
            _modelView = new StudentVisualizationPgViewModel();
            _lecturerId = Session.GetSession().Account.LecturerId;
        }

        //Создание диаграмм

        public async void DrawStudentMarksСhart()
        {
            ChartsStudentExp.Visibility = Visibility.Visible;
            var testsInfo = await _modelView.GetTestsInfoAsync(StudentsCbx.SelectedItem as EF.Student, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? -1);


            // Настраиваем ось X для отображения дат
            var xAxis = new Axis
            {
                Labels = testsInfo.Select(t => t.CreationDate).ToArray(),
                LabelsRotation = 0,
                Name = "Дата сдачи теста",
                LabelsPaint = new SolidColorPaint(SKColors.Black)
            };

            // Создаём серию данных для графика
            var columnSeries = new ColumnSeries<double>
            {
                Values = testsInfo.Select(t => (double)t.Mark).ToArray(),
                Stroke = null,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                IgnoresBarPosition = true,
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    var index = (int)chartPoint.Coordinate.SecondaryValue;
                    var date = xAxis.Labels[index];
                    var mark = chartPoint.Coordinate.PrimaryValue;
                    var presumptiveName = testsInfo.Select(t => t.TestName).ToArray()[index];

                    var resultSelect = testsInfo.Where(
                        t => t.Mark == mark &&
                        t.CreationDate.Contains(date) &&
                        t.TestName == presumptiveName);

                    if (!resultSelect.Any())
                    {
                        return "Неизвестный тест";
                    }
                    return resultSelect.First().TestName;
                }
            };

            // Окрашиваем колонки в зависимости от значения
            columnSeries.OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                var value = point.Coordinate.PrimaryValue;

                if (value > 4)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(60, 179, 113)); // Зеленый
                }
                else if (value > 3)
                {
                    point.Visual.Fill = new SolidColorPaint(new SKColor(140, 203, 95)); // Светло-зеленый
                }
                else if (value > 2)
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
                CustomSeparators = [0, 2, 3, 4, 5],
                MinLimit = 0,
                MaxLimit = 5,
                Name = "Отметка"
            };

            // Применяем настройки к графику
            MarksChart.Series = new ISeries[] { columnSeries, };
            MarksChart.YAxes = new[] { yAxis };
            MarksChart.XAxes = new[] { xAxis };
            MarksChart.UpdateLayout();
        }
        public async void DrawStudentPerformanceСhart()
        {
            ChartsStudentExp.Visibility = Visibility.Visible;
            var testInfo = await _modelView.GetTestsInfoAsync(StudentsCbx.SelectedItem as EF.Student, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? -1);

            // Настраиваем ось X для отображения дат
            var xAxis = new Axis
            {
                Labels = testInfo.Select(t => t.CreationDate).ToArray(),
                LabelsRotation = 0,
                Name = "Дата сдачи теста",
                LabelsPaint = new SolidColorPaint(SKColors.Black)
            };

            // Создаём серию данных для графика
            var columnSeries = new ColumnSeries<double>
            {
                Values = testInfo.Select(t => t.PerformanceRate).ToArray(),
                Stroke = null,
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    var index = (int)chartPoint.Coordinate.SecondaryValue;
                    var date = xAxis.Labels[index];
                    var rate = chartPoint.Coordinate.PrimaryValue;
                    var presumptiveName = testInfo.Select(t => t.TestName).ToArray()[index];
                    var resultSelect = testInfo.Where(
                        t => t.PerformanceRate == rate &&
                        t.CreationDate == date &&
                        t.TestName.Contains(presumptiveName));

                    if (!resultSelect.Any())
                    {
                        return "Неизвестный тест";
                    }
                    return resultSelect.First().TestName;
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
                MinLimit = 0,
                MaxLimit = 100,
                Name = "Результативность (%)"
            };

            // Применяем настройки к графику
            TotalPointsChart.Series = new ISeries[] { columnSeries, };
            TotalPointsChart.YAxes = new[] { yAxis };
            TotalPointsChart.XAxes = new[] { xAxis };
            TotalPointsChart.UpdateLayout();
        }
        public async void DrawStudentAttemptsChart()
        {
            ChartsStudentExp.Visibility = Visibility.Visible;
            var attemptsInfo = await _modelView.GetTestsAttemptsInfoAsync(
                StudentsCbx.SelectedItem as EF.Student,
                SubjectCbx.SelectedItem as EF.Subject,
                _lecturerId??-1);

            var attemptColors = new[]
            {
                new SKColor(60, 179, 113),
                new SKColor(100, 149, 237),
                new SKColor(255, 165, 0),
                new SKColor(220, 20, 60),
                new SKColor(138, 43, 226)
            };

            var series = new List<ISeries<double>>();

            string[] testNames = attemptsInfo.DistinctBy(a => a.TestName).OrderBy(a=>a.CreationDate).Select(a => a.TestName).ToArray();
            List<AttemptInfo>[] attemptInfosMatrix = new List<AttemptInfo>[testNames.Length];

            int maxAttempts = 0;
            for (int i = 0; i < testNames.Length; i++)
            {
                var attemptsByName = attemptsInfo.Where(a => a.TestName == testNames[i]).ToList();
                if (attemptsByName.Count > maxAttempts)
                {
                    maxAttempts = attemptsByName.Count;
                }
                attemptInfosMatrix[i] = attemptsByName;
            }

            for (int i = maxAttempts-1; i >= 0; i--)
            {
                double[] values = new double[testNames.Length];
                for (int j = 0; j < attemptInfosMatrix.Length; j++) {
                    if (attemptInfosMatrix[j].Count-1>=i)
                    {
                        values[j] = attemptInfosMatrix[j][i].PerformanceRate;
                    }
                }
                series.Insert(0, new ColumnSeries<double>
                {
                    Name = $"Попытка №{i + 1}",
                    Values = values,
                    Fill = new SolidColorPaint(attemptColors[i % attemptColors.Length]),
                    Padding = 16
                });
            }

            var xLabels = testNames;

            var xAxis = new Axis
            {
                Labels = xLabels,
                LabelsRotation = 0,
                TextSize = 12,
                LabelsAlignment = LiveChartsCore.Drawing.Align.Middle,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                SeparatorsPaint = new SolidColorPaint(SKColors.Transparent)
            };

            var yAxis = new Axis
            {
                MinLimit = 0,
                MaxLimit = 100,
                Name = "Процент (%)",
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(100))
            };

            AttemptsChart.Series = series;
            AttemptsChart.XAxes = new[] { xAxis };
            AttemptsChart.YAxes = new[] { yAxis };
            AttemptsChart.LegendPosition = LegendPosition.Right;

            AttemptsChart.TooltipPosition = TooltipPosition.Top;
            AttemptsChart.TooltipFindingStrategy = TooltipFindingStrategy.CompareAll;

            AttemptsChart.UpdateLayout();

        }
        public async void DrawStudentAnswersChart()
        {
            AnswersChart.Visibility = Visibility.Visible;

            var answersInfo = await _modelView.GetAnswersInfoAsync(TestNameCbx.SelectedItem as AttemptInfo);

            var attemptColor = new SKColor(97, 81, 164);

            // Создаём серию данных для графика
            var columnSeries = new ColumnSeries<double>
            {
                Values = answersInfo.Select(t => t.PointsTotal*100d/t.PointsMax).ToArray(),
                Fill = new SolidColorPaint(attemptColor),
                Stroke = null,
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    var rate = chartPoint.Coordinate.PrimaryValue;
                    return Math.Round(rate,2)+"%";
                }
            };

            // Настраиваем ось Y
            var yAxis = new Axis
            {
                MinLimit = 0,
                MaxLimit = 100,
                Name = "Набрано балов (%)"
            };

            string[] labels = new string[answersInfo.Length];

            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = (i + 1).ToString();
            }

            // Настраиваем ось X для отображения дат
            var xAxis = new Axis
            {
                Labels = labels,
                LabelsRotation = 0,
                Name = "Номер вопроса",
                LabelsPaint = new SolidColorPaint(SKColors.Black)
            };

            // Применяем настройки к графику
            AnswersChart.Series = new ISeries[] { columnSeries, };
            AnswersChart.YAxes = new[] { yAxis };
            AnswersChart.XAxes = new[] { xAxis };
            AnswersChart.UpdateLayout();
        }

        //Заполнение DataGrid'ов

        private async void WriteTestsMarksTable()
        {
            MarksDtg.Visibility = Visibility.Visible;

            var mainStudentInformation = await _modelView.GetMainStudentInformationAsync(StudentsCbx.SelectedItem as EF.Student, SubjectCbx.SelectedItem as EF.Subject, _lecturerId??0);

            MarksDtg.ItemsSource = mainStudentInformation.TestsInfo;
            MarksDtg.AutoGenerateColumns = false;
            MarksDtg.Columns.Clear();
            MarksDtg.Columns.Add(new DataGridTextColumn { Header = "Тест", Binding = new Binding("TestName") });
            MarksDtg.Columns.Add(new DataGridTextColumn { Header = "Дата сдачи", Binding = new Binding("CreationDate") });
            MarksDtg.Columns.Add(new DataGridTextColumn { Header = "Отметка", Binding = new Binding("Mark") });
        }
        private async void WriteTestsPerformanceTable()
        {
            TotalPointsDtg.Visibility = Visibility.Visible;

            var mainStudentInformation = await _modelView.GetMainStudentInformationAsync(StudentsCbx.SelectedItem as EF.Student, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? 0);

            TotalPointsDtg.ItemsSource = mainStudentInformation.TestsInfo;
            TotalPointsDtg.AutoGenerateColumns = false;
            TotalPointsDtg.Columns.Clear();
            TotalPointsDtg.Columns.Add(new DataGridTextColumn { Header = "Тест", Binding = new Binding("TestName") });
            TotalPointsDtg.Columns.Add(new DataGridTextColumn { Header = "Дата сдачи", Binding = new Binding("CreationDate") });
            TotalPointsDtg.Columns.Add(new DataGridTextColumn { Header = "Результативность (%)", Binding = new Binding("PerformanceRate") });
        }
        private async void WriteStudentAttemptsTable()
        {
            TotalPointsDtg.Visibility = Visibility.Visible;

            var testInfo = await _modelView.GetTestsAttemptsInfoAsync(
                StudentsCbx.SelectedItem as EF.Student,
                SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? -1);

            List<Model.TestAttemptStudentsIndicators> attemptInfos = new List<TestAttemptStudentsIndicators>();

            for (int i = 0; i < testInfo.Length; i++)
            {
                attemptInfos.Add(new TestAttemptStudentsIndicators()
                {
                    TestName = testInfo[i].TestName,
                    CreationDate = testInfo[i].CreationDate.ToShortDateString() + " " + testInfo[i].CreationDate.ToShortTimeString(),
                    AttemptNumber = attemptInfos.Count(a => a.TestName == testInfo[i].TestName) + 1,
                    PerformanceRate = testInfo[i].PerformanceRate,
                    Mark = testInfo[i].Mark
                });
            }
            AttemptsDtg.ItemsSource = attemptInfos;
            AttemptsDtg.AutoGenerateColumns = false;

            AttemptsDtg.Columns.Clear();
            AttemptsDtg.Columns.Add(new DataGridTextColumn { Header = "Тест", Binding = new Binding("TestName") });
            AttemptsDtg.Columns.Add(new DataGridTextColumn { Header = "Попытка (№)", Binding = new Binding("AttemptNumber") });
            AttemptsDtg.Columns.Add(new DataGridTextColumn { Header = "Дата сдачи", Binding = new Binding("CreationDate") });
            AttemptsDtg.Columns.Add(new DataGridTextColumn { Header = "Результативность (%)", Binding = new Binding("PerformanceRate") });
            AttemptsDtg.Columns.Add(new DataGridTextColumn { Header = "Отметка", Binding = new Binding("Mark") });
        }
        private async void WriteStudentAnswersTable()
        {
            AnswersGrd.Visibility = Visibility.Visible;

            var attempt = TestNameCbx.SelectedItem as AttemptInfo;
            var answersInfo = await _modelView.GetAnswersInfoAsync(attempt);

            TestMarkTbx.Text = attempt.Mark.ToString();
            TestPointsScoredTbx.Text = $"{attempt.TotalPoints}/{attempt.MaxTotalPoints}";

            List<AnswerInfo> attemptInfos = new List<AnswerInfo>();

            for (int i = 0; i < answersInfo.Length; i++)
            {
                attemptInfos.Add(new AnswerInfo()
                {
                    Number = i+1,
                    PointsScored = answersInfo[i].PointsTotal,
                    MaxPoints = answersInfo[i].PointsMax,
                    PerformanceRate = Math.Round(answersInfo[i].PointsTotal*100d/answersInfo[i].PointsMax,2)
                });
            }
            AnswersDtg.ItemsSource = attemptInfos;
            AnswersDtg.AutoGenerateColumns = false;

            AnswersDtg.Columns.Clear();
            AnswersDtg.Columns.Add(new DataGridTextColumn { Header = "Вопрос (№)", Binding = new Binding("Number") });
            AnswersDtg.Columns.Add(new DataGridTextColumn { Header = "Балов набрано", Binding = new Binding("PointsScored") });
            AnswersDtg.Columns.Add(new DataGridTextColumn { Header = "Максимум балов", Binding = new Binding("MaxPoints") });
            AnswersDtg.Columns.Add(new DataGridTextColumn { Header = "Результативность (%)", Binding = new Binding("PerformanceRate") });
        }


        //Обработчики событий

        private async void StudentPage_Loaded(object sender, RoutedEventArgs e)
        {
            GroupNameCbx.ItemsSource = await _modelView.GetGroupsNameAsync();
        }
        private async void GroupNameCbx_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (GroupNameCbx.SelectedValue is null)
            {
                DisableComboBox(StudentsCbx);
                return;
            }
            try
            {
                StudentsCbx.IsEnabled = true;
                var source = await _modelView.GetStudentsNameByGroupAsync(GroupNameCbx.SelectedValue.ToString());
                if (source.Count == 0)
                {
                    DisableComboBox(StudentsCbx);
                    return;
                }
                StudentsCbx.ItemsSource = source;
            }
            catch (Exception)
            {
                DisableComboBox(StudentsCbx);
            }
        }

        private async void StudentsCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupNameCbx.SelectedValue is null)
            {
                DisableComboBox(SubjectCbx);
                return;
            }
            try
            {
                SubjectCbx.IsEnabled = true;
                var source = await _modelView.GetSubjectsByStudentAsync(StudentsCbx.SelectedItem as EF.Student,_lecturerId ?? -1);
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
                DisableComboBox(TestNameCbx);
                return;
            }
            try
            {
                TestNameCbx.IsEnabled = true;
                var source = await _modelView.GetTestsAttemptsInfoAsync(StudentsCbx.SelectedItem as EF.Student, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? -1);
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


            WriteMainStudentInformation();

            DrawStudentMarksСhart();
            WriteTestsMarksTable();

            DrawStudentPerformanceСhart();
            WriteTestsPerformanceTable();

            DrawStudentAttemptsChart();
            WriteStudentAttemptsTable();
        }

        private void TestNameCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(TestNameCbx.SelectedItem is AttemptInfo))
            {
                AnswersChart.Visibility = Visibility.Collapsed;
                AnswersGrd.Visibility = Visibility.Collapsed;
                return;
            }

            DrawStudentAnswersChart();
            WriteStudentAnswersTable();
        }

        private void AvgMarkRefInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Средняя оценка студента по всем тестам. Рассчитывается как среднее арифметическое последних полученных оценок за каждый тест.", "Краткая справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void AvgPointsRatio_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Средний процент правильно решенных заданий по всем тестам. Показывает общую успешность студента в выполнении тестов.", "Краткая справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void DinamicValue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Отношение результативности последнего теста к предпоследнему. Показывает, улучшилась или ухудшилась успеваемость студента в сравнении с предыдущей попыткой.", "Краткая справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Управление UI
        private void DisableComboBox(ComboBox comboBox)
        {
            comboBox.ItemsSource = null;
            comboBox.Items.Clear();
            comboBox.IsEnabled = false;
        }

        private async void WriteMainStudentInformation()
        {
            GeneralStudentInfoExp.Visibility = Visibility.Visible;
            var mainStudentInformation = await _modelView.GetMainStudentInformationAsync(StudentsCbx.SelectedItem as EF.Student, SubjectCbx.SelectedItem as EF.Subject, _lecturerId ?? 0);
            AvgMarkTbx.Text = mainStudentInformation.AvgMark.ToString();

            if (mainStudentInformation.AvgMark < 3.5)
            {
                AvgMarkTbx.Foreground = Brushes.Red;
            }
            else if (mainStudentInformation.AvgMark > 4.5)
            {
                AvgMarkTbx.Foreground = Brushes.Green;
            }
            else
            {
                AvgMarkTbx.Foreground = Brushes.Black;
            }

            AvgPointsRatioTbx.Text = mainStudentInformation.AvgPointsRatio.ToString() + "%";
            if (mainStudentInformation.AvgPointsRatio < 65)
            {
                AvgPointsRatioTbx.Foreground = Brushes.Red;
            }
            else if (mainStudentInformation.AvgPointsRatio >= 80)
            {
                AvgPointsRatioTbx.Foreground = Brushes.Green;
            }
            else
            {
                AvgPointsRatioTbx.Foreground = Brushes.Black;
            }

            if (mainStudentInformation.DinamicValue != null)
            {
                DinamicValueTbx.Text = mainStudentInformation.DinamicValue.ToString() + "%";
                if (mainStudentInformation.DinamicValue < 0)
                {
                    DinamicValueTbx.Foreground = Brushes.Red;
                }
                else if (mainStudentInformation.DinamicValue > 15)
                {
                    DinamicValueTbx.Foreground = Brushes.Green;
                }
                else
                {
                    DinamicValueTbx.Foreground = Brushes.Black;
                }
            }
            else
            {
                DinamicValueTbx.Text = "Нет данных";
                DinamicValueTbx.Foreground = Brushes.Black;
            }
        }
    }
}
