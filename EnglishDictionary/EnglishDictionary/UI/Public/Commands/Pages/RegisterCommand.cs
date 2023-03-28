using EnglishDictionary.Models;
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
    public class RegisterCommand
    {

        private RegisterViewModel _viewModel;
        public RegisterCommand(RegisterViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.RedirectToSignInCommand = new ReplayCommand(RedirectToSignIn);
            _viewModel.SignUpCommand = new ReplayCommand(SignUp);
            _viewModel.SignUpWithGoogleCommand = new ReplayCommand(SignUpWithGoogle);
        }

        private void SignUp(object parameter)
        {


            Frame frame = (Frame)Application.Current.MainWindow.FindName("frameContent");

            if (String.IsNullOrEmpty(_viewModel.Email) || String.IsNullOrEmpty(_viewModel.Password)
                || String.IsNullOrEmpty(_viewModel.FullName) || String.IsNullOrEmpty(_viewModel.ConfirmPassword))
            {
                MessageBox.Show("Please enter all required feilds");
            }
            else
            {

                using (var context = new DictionaryContext())
                {
                    Account account = new Account();
                    account.Username = _viewModel.Email;
                    account.Password = _viewModel.Password;
                    account.Role = "User";


                    context.Accounts.Add(account);
                    if (context.SaveChanges() > 0)
                    {

                        RedirectToSignIn(parameter);
                    }
                    else
                    {
                        MessageBox.Show("Failed to sign up new account");
                    }

                }


            }


        }


        private void RedirectToSignIn(object parameter)
        {
            Frame frame = (Frame)Application.Current.MainWindow.FindName("frameContent");
            frame.Navigate(new LoginView());
        }

        private void SignUpWithGoogle(object parameter) { }

    }
}
