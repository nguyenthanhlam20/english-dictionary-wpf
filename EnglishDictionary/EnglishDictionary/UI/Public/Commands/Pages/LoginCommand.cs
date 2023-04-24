

using EnglishDictionary.Models;
using EnglishDictionary.Properties;
using EnglishDictionary.UI.Admin;
using EnglishDictionary.UI.User;
using FinancialWPFApp.UI.Public.ViewModels.Pages;
using FinancialWPFApp.UI.Public.Views.Pages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace FinancialWPFApp.UI.Public.Commands.Pages
{
    public class LoginCommand
    {
        private LoginViewModel _viewModel;


        public LoginCommand(LoginViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.RedirectToSignUpCommand = new ReplayCommand(RedirectToSignUp);
            _viewModel.SignInCommand = new ReplayCommand(SignIn);
        }

        private void SignIn(object parameter)
        {
            Frame frame = (Frame)Application.Current.MainWindow.FindName("frameContent");

            if (String.IsNullOrEmpty(_viewModel.Username) || String.IsNullOrEmpty(_viewModel.Password))
            {
                MessageBox.Show("Please enter username and password");
            }
            else
            {


                using (var context = new DictionaryContext())
                {
                    Account account = context.Accounts.SingleOrDefault(u => u.Username == _viewModel.Username && u.Password == _viewModel.Password);
                    if (account != null)
                    {

                        Settings.Default.Username = account.Username;
                        Settings.Default.Password = account.Password;
                        Settings.Default.Role = account.Role;

                        // Save the settings
                        Settings.Default.Save();
                        Application.Current.MainWindow.Hide();
                        if (account.Role.ToLower() == "admin")
                        {

                            AdminMainWindow adminWindow = new AdminMainWindow();
                            Application.Current.MainWindow = adminWindow;
                            adminWindow.Show();
                        }
                        else
                        {
                            UserMainWindow window = new UserMainWindow();
                            Application.Current.MainWindow = window;
                            window.Show();

                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("Incorrect username or password");

                    }


                }

            }


        }
        private void RedirectToSignUp(object parameter)
        {
            Frame frame = (Frame)Application.Current.MainWindow.FindName("frameContent");
            frame.Navigate(new RegisterView());
        }

        private void SignInWithGoogle(object parameter) { }


    }
}
