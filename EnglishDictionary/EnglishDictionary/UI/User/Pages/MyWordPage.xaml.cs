using EnglishDictionary.Models;
using FinancialWPFApp.UI;
using MahApps.Metro.IconPacks;
using Microsoft.EntityFrameworkCore;
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

namespace EnglishDictionary.UI.User.Pages
{
    /// <summary>
    /// Interaction logic for MyWordPage.xaml
    /// </summary>
    public partial class MyWordPage : Page
    {
        private int _targetWordId;
        private string filterSearch = "";
        private string filterStart = "";
        private StackPanel sp;
        private TextBlock tb;

        public ReplayCommand SaveWordCommand { get; set; }

        public List<Button> _alphabetButtons = new List<Button>();
        public MyWordPage()
        {
            InitializeComponent();
            SaveWordCommand = new ReplayCommand(SaveWord);

            RegisterButtons();
            DataContext = this;
        }


        public void RegisterButtons()
        {
            _alphabetButtons = new List<Button>() { btnAll, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z };
        }
        public void SaveWord(object parameter)
        {
            try
            {
                using (var context = new DictionaryContext())
                {
                    int wordId = (int)parameter;

                    Word word = context.Words.SingleOrDefault(w => w.WordId == wordId);
                    if (word.IsUserSaved == false)
                    {
                        word.IsUserSaved = true;
                        word.UserSavedIconName = "ContentSaveCheck";
                    }
                    else
                    {
                        word.IsUserSaved = false;
                        word.UserSavedIconName = "ContentSaveOff";
                    }
                    context.Words.Update(word);

                    context.SaveChanges();

                    SearchWords();
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error saving word");
            }

        }
        private void LoadAllWords()
        {
            ResetColorButton();
            txtSearch.Clear();
            btnAll.Background = Brushes.Gray;
            resultContainer.Visibility = Visibility.Visible;

            lbResult.Visibility = Visibility.Visible;

            List<Word> words = GetAllWords();

            if (words.Count() > 0)
            {
                lbResult.Content = $"Total {words.Count()} saved words";
                dgWords.ItemsSource = words;
                resultContainer.Visibility = Visibility.Visible;
            }
            else
            {
                lbResult.Content = $"No words found";
                resultContainer.Visibility = Visibility.Hidden;
                vocabulary.Visibility = Visibility.Hidden;
            }
        }



        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;

                filterSearch = txt.Text;
                if (String.IsNullOrEmpty(txt.Text) == false)
                {
                    btnAll.Background = Brushes.BlueViolet;

                    btnClearSearch.Visibility = Visibility.Visible;

                    SearchWords();
                }
                else
                {
                    btnClearSearch.Visibility = Visibility.Hidden;
                    ResetColorButton();
                    LoadAllWords();
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Cannot search");
            }
        }

        public void SearchWords()
        {
            lbResult.Visibility = Visibility.Visible;

            List<Word> words = GetResult();
            int total = GetAllWords().Count();

            if (words.Count() > 0)
            {
                if (btnAll.Background == Brushes.Gray)
                {
                    lbResult.Content = $"Total {words.Count()} saved words";
                   
                }
                else
                {
                    lbResult.Content = $"Found {words.Count()} words";
                }
                dgWords.ItemsSource = words;
                resultContainer.Visibility = Visibility.Visible;
            }
            else
            {
                lbResult.Content = $"No words found";
                resultContainer.Visibility = Visibility.Hidden;
                vocabulary.Visibility = Visibility.Hidden;

            }
        }

        public List<Word> GetResult()
        {
            using (var context = new DictionaryContext())
            {
                if (String.IsNullOrEmpty(filterStart) == false)
                {
                    return context.Words.Include(w => w.Type).Where(w => ((w.WordName.ToUpper().StartsWith(filterStart) && w.WordName.Contains(filterSearch))
               || filterSearch.Contains(w.WordName)) && w.IsUserSaved == true).ToList();
                }
                else
                {
                    return context.Words.Include(w => w.Type).Where(w => (w.WordName.Contains(filterSearch)
              || filterSearch.Contains(w.WordName)) && w.IsUserSaved == true).ToList();
                }
            }
        }

        public List<Word> GetAllWords()
        {
            using (var context = new DictionaryContext())
            {
                return context.Words.Include(w => w.Type).Where(w => w.IsUserSaved == true).ToList();
            }
        }

        private void LoadWordDetails()
        {
            try
            {
                vocabulary.Visibility = Visibility.Visible;
                using (var context = new DictionaryContext())
                {
                    Word word = context.Words.Include(w => w.Type).SingleOrDefault(w => w.WordId == _targetWordId);
                    if (word != null)
                    {
                        GenerateWordDetails(word);
                    }
                    List<WordExample> examples = context.WordExamples.Where(we => we.WordId == _targetWordId).ToList();
                    GenerateWordExample(examples);

                    List<WordMeaning> meanings = context.WordMeanings.Where(we => we.WordId == _targetWordId).ToList();
                    GenerateWordMeaning(meanings);

                }
            }
            catch (Exception)
            {

                MessageBox.Show("Cannot show word details");
            }
        }

        private void GenerateWordDetails(Word word)
        {
            lbWordName.Text = word.WordName;
            lbIPA.Text = $"[ {word.IPA} ]";
            lbWordType.Content = word.Type.WordTypeName;
        }



        private void GenerateWordExample(List<WordExample> examples)
        {
            exampleContainer.Children.Clear();

            if (examples.Count > 0)
            {
                txtExample.Text = examples.ElementAt(0).ExampleContent;
            }
            foreach (WordExample we in examples)
            {
                // Create row container
                sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Margin = new Thickness(0, 0, 0, 10);
                // Create icon
                var icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Circle;
                icon.Foreground = new SolidColorBrush(Colors.YellowGreen);
                icon.Width = 10;
                icon.Height = 10;
                icon.VerticalAlignment = VerticalAlignment.Center;
                icon.Margin = new Thickness(0, 2, 10, 0);

                // Create text block for containing text
                tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 14;
                tb.MaxWidth = 680;
                tb.Text = we.ExampleContent;

                // Add icon and text block to row container
                sp.Children.Add(icon);
                sp.Children.Add(tb);

                // Add to list
                exampleContainer.Children.Add(sp);
            }

        }
        private void GenerateWordMeaning(List<WordMeaning> meanings)
        {
            meaningContainer.Children.Clear();
            if (meanings.Count > 0)
            {
                txtMeaning.Text = meanings.ElementAt(0).MeaningContent;
            }
            foreach (WordMeaning we in meanings)
            {
                // Create row container
                sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Margin = new Thickness(0, 0, 0, 10);

                // Create icon
                var icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Circle;
                icon.Foreground = new SolidColorBrush(Colors.YellowGreen);
                icon.Width = 10;
                icon.Height = 10;
                icon.VerticalAlignment = VerticalAlignment.Center;
                icon.Margin = new Thickness(0, 1, 10, 0);

                // Create text block for containing text
                tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 14;
                tb.Text = we.MeaningContent;
                tb.MaxWidth = 680;

                // Add icon and text block to row container
                sp.Children.Add(icon);
                sp.Children.Add(tb);

                // Add to list
                meaningContainer.Children.Add(sp);
            }
        }



        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
        }

        private void dgWords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid lv = (DataGrid)sender;

                Word word = lv.SelectedItem as Word;
                if (word != null)
                {
                    _targetWordId = word.WordId;

                    LoadWordDetails();
                }
            }
            catch (Exception)
            {


            }

        }

        private void btnAlphabetSearch_Click(object sender, RoutedEventArgs e)
        {
            ResetColorButton();

            Button btn = sender as Button;
            btn.Background = Brushes.Gray;

            filterStart = btn.Name;
            SearchWords();

        }

        private void ResetColorButton()
        {
            foreach (Button btn in _alphabetButtons)
            {
                btn.Background = Brushes.BlueViolet;
            }
        }


        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            ResetColorButton();
            Button btn = sender as Button;
            btn.Background = Brushes.Gray;




            filterStart = "";
            txtSearch.Clear();
            LoadAllWords();
        }
    }
}
