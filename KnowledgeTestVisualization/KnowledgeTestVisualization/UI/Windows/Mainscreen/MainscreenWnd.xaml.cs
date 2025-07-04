using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KnowledgeTestVisualization.EF;
using KnowledgeTestVisualization.ViewModel;
using KnowledgeTestVisualization.UI.Pages;

namespace KnowledgeTestVisualization.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainscreenWnd.xaml
    /// </summary>
    public partial class MainscreenWnd : Window
    {
        public MainscreenWnd()
        {
            InitializeComponent();
        }

        public Pages.WelcomePg WelcomePg { get; private set; }
        public Pages.StudentVisualizationPg StudentVisualizationPg { get; set; } = new StudentVisualizationPg();
        public Pages.GroupVisualizationPg GroupVisualizationPg { get; set; } = new GroupVisualizationPg();
        public bool IsSessionInterrupting { get; set; } = false;

        private void StudentMI_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(StudentVisualizationPg);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Pages.WelcomePg WelcomePg = new WelcomePg(this);
            MainFrame.Navigate(WelcomePg);
        }
        private void GroupMI_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(GroupVisualizationPg);
        }

        private void ReturnToWelcomePageMI_Click(object sender, RoutedEventArgs e)
        {
            Pages.WelcomePg WelcomePg = new WelcomePg(this);
            MainFrame.Navigate(WelcomePg);
        }

        private void ChangeUserSpl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти из учетной записи?", "Выход из системы", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                IsSessionInterrupting = true;
                Close();
            }
        }
    }
}
