using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KnowledgeTestVisualization.UI
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWnd.xaml
    /// </summary>
    public partial class AuthorizationWnd : Window
    {
        ViewModel.AuthorizationWndViewModel _modelView;
        public AuthorizationWnd()
        {
            InitializeComponent();
            _modelView = new ViewModel.AuthorizationWndViewModel(this);
        }
        public string Login
        {
            get
            {
                return LoginTbx.Text;
            }
        }
        public string Password
        {
            get
            {
                if (PasswordIsShown)
                {
                    return PasswordTbx.Text;
                }
                else
                {
                    return PasswordPbx.Password;
                }
            }
        }

        private bool PasswordIsShown = false;
        private void PasswordViewPictogramImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PasswordIsShown = !PasswordIsShown;
            if (PasswordIsShown)
            {
                PasswordPbx.Visibility = Visibility.Collapsed;
                PasswordTbx.Visibility = Visibility.Visible;
                PasswordTbx.Text = PasswordPbx.Password;
                PasswordViewPictogramImg.Source = new BitmapImage(new Uri("/Resources/view.png", UriKind.Relative));
            }
            else
            {
                PasswordPbx.Visibility = Visibility.Visible;
                PasswordTbx.Visibility = Visibility.Collapsed;
                PasswordPbx.Password = PasswordTbx.Text;
                PasswordViewPictogramImg.Source = new BitmapImage(new Uri("/Resources/hidden.png", UriKind.Relative));
            }
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_modelView.CheckRequiredField())
            {
                return;
            }
            await _modelView.AuthorizeAcync(Login, Password);
        }

        private void RestorePasswordBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Для восстановления доступа к вашей учетной записи обратитесь к системному администратору.", "Восстановление доступа", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите закрыть программу?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.Yes) 
            {
                Close();
            }
        }
    }
}
