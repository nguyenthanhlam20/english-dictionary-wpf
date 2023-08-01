using EnglishDictionary.UI.Admin.Pages;
using FinancialWPFApp.UI.Public.Views;
using System;
using System.Windows;

namespace EnglishDictionary.UI.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public double ScreenHeight { get; set; } = 0f;

        private HomePage homePage;
        private SavedWordPage saveWordPage;

        public enum Page
        {
            HomePage,
            SaveWordPage,
        }

        public Page currentPage = Page.HomePage;
        public AdminMainWindow()
        {
            ScreenHeight = SystemParameters.PrimaryScreenHeight - 250;
            InitializeComponent();
            homePage = new HomePage();
            frameContent.Content = homePage;
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
            homePage = new HomePage();
            lbTitle.Content = "Home";
            currentPage = Page.HomePage;
            frameContent.Content = homePage;
            ResizeTable();
        }

        private void rdSavedWord_Click(object sender, RoutedEventArgs e)
        {
            lbTitle.Content = "Saved Word";
            currentPage = Page.SaveWordPage;
            saveWordPage = new SavedWordPage();
            frameContent.Content = saveWordPage;
            ResizeTable();

        }

        private void rdSettings_Click(object sender, RoutedEventArgs e)
        {
            AdminSettingPage page = new AdminSettingPage();
            lbTitle.Content = "Settings";

            frameContent.Content = page;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            ResizeTable();
        }

        private void ResizeTable()
        {
            if (!double.IsNaN(this.ActualHeight))
            {
                switch (currentPage)
                {
                    case Page.HomePage:
                        homePage.ResizeTable(this.ActualHeight);
                        break;
                    case Page.SaveWordPage:
                        saveWordPage.ResizeTable(this.ActualHeight);
                        break;

                }

            }

        }
    }
}
