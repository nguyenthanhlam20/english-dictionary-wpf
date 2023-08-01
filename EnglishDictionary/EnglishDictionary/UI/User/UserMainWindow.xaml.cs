using EnglishDictionary.UI.User.Pages;
using FinancialWPFApp.UI.Public.Views;
using System.ComponentModel;
using System.Windows;

namespace EnglishDictionary.UI.User
{
    /// <summary>
    /// Interaction logic for UserMainWindow.xaml
    /// </summary>
    public partial class UserMainWindow : Window, INotifyPropertyChanged
    {

        public enum Page
        {
            DictionaryPage,
            MyWordPage,
        }

        private Page currentPage = Page.DictionaryPage;

        public double _screenHeight = 0f;

        public double ScreenHeight
        {
            get { return _screenHeight; }
            set
            {
                _screenHeight = value;
                OnPropertyChanged("ScreenHeight");
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DictionaryPage dictionaryPage;
        private MyWordPage myWordPage;
        public UserMainWindow()
        {
            ScreenHeight = SystemParameters.PrimaryScreenHeight - 250;
            InitializeComponent();

            dictionaryPage = new DictionaryPage();

            frameContent.Content = dictionaryPage;

            DataContext = this;
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
            myWordPage = new MyWordPage();
            myWordPage.ResizeTable(this.ActualHeight);
            currentPage = Page.MyWordPage;
            lbTitle.Content = "History";
            frameContent.Content = myWordPage;
        }

        private void rdDictionary_Click(object sender, RoutedEventArgs e)
        {
            dictionaryPage = new DictionaryPage();
            dictionaryPage.ResizeTable(this.ActualHeight);
            currentPage = Page.DictionaryPage;

            lbTitle.Content = "Dictionary";

            frameContent.Content = dictionaryPage;
        }

        private void rdSettings_Click(object sender, RoutedEventArgs e)
        {
            UserSettingPage page = new UserSettingPage();
            lbTitle.Content = "Settings";

            frameContent.Content = page;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            switch(currentPage)
            {
                case Page.DictionaryPage:
                    dictionaryPage.ResizeTable(this.ActualHeight);
                    break;
                case Page.MyWordPage:
                    myWordPage.ResizeTable(this.ActualHeight);
                    break;
            }
        }
    }
}



