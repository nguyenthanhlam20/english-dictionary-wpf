using CsvHelper;
using CsvHelper.Configuration;
using EnglishDictionary.Models;
using EnglishDictionary.UI.Admin.Pages;
using EnglishDictionary.UI.User.Pages;
using FinancialWPFApp.UI;
using FinancialWPFApp.UI.Public.Views;
using MahApps.Metro.IconPacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.X86;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EnglishDictionary.UI.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public double ScreenHeight { get; set; }
        public AdminMainWindow()
        {
            ScreenHeight = SystemParameters.PrimaryScreenHeight - 100;
            InitializeComponent();
            HomePage page = new HomePage();
            frameContent.Content = page;
            lbTitle.Content = "Home";

            DataContext = this;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown(); // Exit the application
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Hide();

            MainWindowView window = new MainWindowView();


            Application.Current.MainWindow = window;
            Application.Current.MainWindow.Show();
        }


        private void rdHome_Click(object sender, RoutedEventArgs e)
        {
            HomePage page = new HomePage();
            lbTitle.Content = "Home";

            frameContent.Content = page;
        }

        private void rdSavedWord_Click(object sender, RoutedEventArgs e)
        {
            lbTitle.Content = "Saved Word";

            SavedWordPage page = new SavedWordPage();
            frameContent.Content = page;
        }

        private void rdSettings_Click(object sender, RoutedEventArgs e)
        {
            AdminSettingPage page = new AdminSettingPage();
            lbTitle.Content = "Settings";

            frameContent.Content = page;
        }
    }
}
