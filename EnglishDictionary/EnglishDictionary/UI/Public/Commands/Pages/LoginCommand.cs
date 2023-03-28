

using EnglishDictionary.Models;
using EnglishDictionary.UI.Admin;
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
            _viewModel.SignInWithGoogleCommand = new ReplayCommand(SignInWithGoogle);
            _viewModel.RedirectToForgotPasswordCommand = new ReplayCommand(RedirectToForgotPassword);
        }

        private void SignIn(object parameter)
        {
            Frame frame = (Frame)Application.Current.MainWindow.FindName("frameContent");

            if (String.IsNullOrEmpty(_viewModel.Email) || String.IsNullOrEmpty(_viewModel.Password))
            {
                MessageBox.Show("Please enter email and password");
            }
            else
            {


                using (var context = new DictionaryContext())
                {
                    Account account = context.Accounts.SingleOrDefault(u => u.Username == _viewModel.Email && u.Password == _viewModel.Password);
                    if (account != null)
                    {
                        Application.Current.MainWindow.Hide();
                        if (account.Role == "Admin")
                        {

                            AdminMainWindow window = new AdminMainWindow();
                            Application.Current.MainWindow = window;

                        }
                        else
                        {
                            AdminMainWindow window = new AdminMainWindow();
                            Application.Current.MainWindow = window;


                        }
                        Application.Current.MainWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect email or password");

                    }


                }

            }


        }

        private void RedirectToForgotPassword(object parameter)
        {
           
        }

        private void RedirectToSignUp(object parameter)
        {
            Frame frame = (Frame)Application.Current.MainWindow.FindName("frameContent");
            frame.Navigate(new RegisterView());
        }

        private void SignInWithGoogle(object parameter) { }


    }
}
