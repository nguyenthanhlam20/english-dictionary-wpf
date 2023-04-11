using EnglishDictionary.Models;
using EnglishDictionary.UI.User.Pages;
using FinancialWPFApp.UI.Public.Views;
using MahApps.Metro.IconPacks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EnglishDictionary.UI.User
{
    /// <summary>
    /// Interaction logic for UserMainWindow.xaml
    /// </summary>
    public partial class UserMainWindow : Window
    {
        public UserMainWindow()
        {
            InitializeComponent();

            DictionaryPage page = new DictionaryPage();

            frameContent.Content = page;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Hide();

            MainWindowView window = new MainWindowView();


            Application.Current.MainWindow = window;
            Application.Current.MainWindow.Show();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void rdMyWord_Click(object sender, RoutedEventArgs e)
        {
            MyWordPage page = new MyWordPage();
            lbTitle.Content = "My Words";
            frameContent.Content = page;
        }

        private void rdDictionary_Click(object sender, RoutedEventArgs e)
        {
            DictionaryPage page = new DictionaryPage();
            lbTitle.Content = "Dictionary";

            frameContent.Content = page;
        }
    }
}
