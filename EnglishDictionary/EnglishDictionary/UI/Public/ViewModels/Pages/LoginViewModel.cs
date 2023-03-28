using FinancialWPFApp.UI.Public.Commands.Pages;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FinancialWPFApp.UI.Public.ViewModels.Pages
{
    public class LoginViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        [NotNull]
        public ReplayCommand SignInCommand { get; set; }

        [NotNull]
        public ReplayCommand SignInWithGoogleCommand { get; set; }

        [NotNull]
        public ReplayCommand RedirectToSignUpCommand { get; set; }

        [NotNull]
        public ReplayCommand RedirectToForgotPasswordCommand { get; set; }

        [NotNull]
        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        [NotNull]
        private bool _isRemember;
        public bool IsRemember
        {
            get { return _isRemember; }
            set
            {
                _isRemember = value;
                OnPropertyChanged("IsRemember");
            }
        }

        [NotNull]
        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }


        public LoginViewModel()
        {
            Email= string.Empty;
            LoginCommand commands = new LoginCommand(this);
        }


    }
}
