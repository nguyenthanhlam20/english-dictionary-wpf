using EnglishDictionary.Models;
using FinancialWPFApp.UI.Public.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace FinancialWPFApp.UI.Public.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : System.Windows.Controls.Page
    {

        private LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

        }


        public void InitializeWordType()
        {
            using (var context = new DictionaryContext())
            {
                List<WordType> words = new List<WordType>()
                {

                     new WordType() { WordTypeName = "noun" },
                     new WordType() { WordTypeName = "verb" },
                     new WordType() { WordTypeName = "adverb" },
                     new WordType() { WordTypeName = "pronoun" },
                     new WordType() { WordTypeName = "adjective" },
                     new WordType() { WordTypeName = "determiner" },
                     new WordType() { WordTypeName = "determiner" },
                     new WordType() { WordTypeName = "preposition" },
                  };


                foreach (WordType w in words)
                {
                    context.WordTypes.Add(w);
                }

                int chages = context.SaveChanges();

                if (chages > 0)
                {
                    MessageBox.Show($"Save {chages} successful");
                }

            }
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = (sender as PasswordBox).Password;

        }
    }
}
