using FinancialWPFApp.UI.Public.Views.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialWPFApp.UI.Public.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public object _currentPage;
        public object CurrentPage
        { 
            get { return _currentPage; }
            set
            {
                _currentPage= value;
                OnPropertyChanged("CurrentPage");

            }
        }

      
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public MainWindowViewModel()
        {
            CurrentPage = new LoginView();
        }
    }
}
