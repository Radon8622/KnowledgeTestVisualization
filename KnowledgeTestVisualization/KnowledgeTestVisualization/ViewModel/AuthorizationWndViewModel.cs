using System;
using System.Collections.Generic;
using System.Text;
using KnowledgeTestVisualization.Model;
using KnowledgeTestVisualization.UI;
using System.Text.RegularExpressions;
using System.Windows;
using KnowledgeTestVisualization.UI.Windows;
using System.Threading.Tasks;

namespace KnowledgeTestVisualization.ViewModel
{
    class AuthorizationWndViewModel
    {
        AuthorizationWnd _view;
        public AuthorizationWndViewModel(AuthorizationWnd authorizationWnd) 
        {
            _view = authorizationWnd;
        }
        public async Task AuthorizeAcync(string login, string password)
        {
            _view.loginSpl.IsEnabled = false;
            var authorizated = await AuthorizationManager.AuthorizeAsync(login, password);

            switch (authorizated)
            {
                case AuthorizeStatus.Fine:
                    MainscreenWnd mainscreenWnd = new MainscreenWnd();
                    _view.Hide();
                    mainscreenWnd.Closed += MainscreenWnd_Closed;
                    mainscreenWnd.Show();
                    break;

                case AuthorizeStatus.Fail:
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;

                case AuthorizeStatus.Error:
                    MessageBox.Show("Ошибка подключения к БД. Обратитесь за помощью к системному администратору.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                    App.Current.Shutdown();
                    return;
            }
            _view.loginSpl.IsEnabled = true;
        }

        private void MainscreenWnd_Closed(object sender, EventArgs e)
        {
            if ((sender as MainscreenWnd).IsSessionInterrupting) {
                Session.DestroySession();
                _view.Show();
                return;
            }
            _view.Close();
        }

        public bool CheckRequiredField()
        {
            if (string.IsNullOrWhiteSpace(_view.Login)) 
            {
                MessageBox.Show("Вы не ввели логин. Пожалуйста, сделайте это и повторите попытку входа.","Некоректный логин");
                return false;
            }

            string pattern = @"^[a-zA-Z0-9_]{3,20}$";
            if (!Regex.IsMatch(_view.Login, pattern))
            {
                MessageBox.Show("Логин должен:\n1) содержать только латинские буквы (a-z, A-Z), цифры (0-9) и символы подчёркивания; \n2) иметь длину от 3 до 20 символов.\n\nПожалуйста, проверьте введенный логин на соотвествие этим требованиям и повторите попытку входа.", "Некоректный логин");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_view.Password))
            {
                MessageBox.Show("Вы не ввели пароль. Пожалуйста, сделайте это и повторите попытку входа.", "Некоректный пароль");
                return false;
            }

            pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d!@#$%^&*]{8,}$";
            if (!Regex.IsMatch(_view.Password, pattern))
            {
                MessageBox.Show("Пароль должен:" +
                    "\n1) содержать минимум 8 символов;" +
                    "\n2) содержать минимум одну заглавную букву;" +
                    "\n2) содержать минимум одну строчную букву;" +
                    "\n3) содержать минимум одну цифру;" +
                    "\n4) содержать только латинские буквы (a-z, A-Z), цифры (0-9).",
                    "Некоректный логин");
                return false;
            }

            return true;
        }
    }
}
