using KnowledgeTestVisualization.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KnowledgeTestVisualization.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для WelcomePg.xaml
    /// </summary>
    public partial class WelcomePg : Page
    {
        private MainscreenWnd _mainscreenWndView;
        public WelcomePg(MainscreenWnd mainscreenWnd)
        {
            _mainscreenWndView = mainscreenWnd;
            InitializeComponent();
        }

        private void CheckIndividualStatisticsTbk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mainscreenWndView.MainFrame.Navigate(_mainscreenWndView.StudentVisualizationPg);
        }

        private void CheckGroupStatisticsTbk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mainscreenWndView.MainFrame.Navigate(_mainscreenWndView.GroupVisualizationPg);
        }
    }
}
