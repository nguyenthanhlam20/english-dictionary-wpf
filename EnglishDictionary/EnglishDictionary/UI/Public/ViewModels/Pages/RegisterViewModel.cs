using FinancialWPFApp.UI.Public.Commands.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialWPFApp.UI.Public.ViewModels.Pages
{
    public class RegisterViewModel : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        [MaybeNull]
        public ReplayCommand SignUpCommand { get; set; }

        [MaybeNull]
        public ReplayCommand SignUpWithGoogleCommand { get; set; }

        [MaybeNull]
        public ReplayCommand RedirectToSignInCommand { get; set; }


        [NotNull]
        public string _email { get; set; }

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
        public string _password { get; set; }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
        [NotNull]
        public string _confirmPassword { get; set; }

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged("ConfirmPassword");
            }
        }
        [NotNull]
        public string _fullname { get; set; }

        public string FullName
        {
            get { return _fullname; }
            set
            {
                _fullname = value;
                OnPropertyChanged("FullName");
            }
        }


        public RegisterViewModel()
        {
            RegisterCommand commands = new RegisterCommand(this);
        }
    }
}
