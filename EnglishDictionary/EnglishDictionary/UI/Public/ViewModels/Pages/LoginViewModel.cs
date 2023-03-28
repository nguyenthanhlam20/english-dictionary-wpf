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
        public ReplayCommand RedirectToSignUpCommand { get; set; }


        [NotNull]
        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("Email");
            }
        }


        public string Password { get; set; }


        public LoginViewModel()
        {
            Username = string.Empty;
            LoginCommand commands = new LoginCommand(this);
        }


    }
}
