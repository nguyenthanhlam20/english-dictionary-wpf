using EnglishDictionary.Models;
using FinancialWPFApp.UI;
using MahApps.Metro.IconPacks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishDictionary.UI.User.Pages
{
    /// <summary>
    /// Interaction logic for DictionaryPage.xaml
    /// </summary>
    public partial class DictionaryPage : Page
    {
        private int _targetWordId;
        private string filterSearch = "";
        private string filterStart = "";
        private StackPanel sp;
        private TextBlock tb;

        public ReplayCommand SaveWordCommand { get; set; }

        public List<Button> _alphabetButtons = new List<Button>();
        public double ScreenHeight { get; set; }
        public double ScreenWidth { get; set; }

        public List<Word> defaultWords = new();
        public DictionaryPage()
        {
            ScreenHeight = SystemParameters.PrimaryScreenHeight - 480;
            ScreenWidth = SystemParameters.PrimaryScreenWidth - 856;
            InitializeComponent();
            SaveWordCommand = new ReplayCommand(SaveWord);
            RegisterButtons();
            DataContext = this;

            AssignDefaultWords();
            LoadAllWords();
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

                    dgWords.ItemsSource = GetResult();
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

            btnAll.Background = Brushes.Gray;
            txtSearch.Clear();
            dgWords.Visibility = Visibility.Visible;

            lbResult.Visibility = Visibility.Visible;

            
            if (defaultWords.Count() > 0)
            {
                lbResult.Content = $"Total {defaultWords.Count()} words";
                dgWords.ItemsSource = defaultWords;
                dgWords.Visibility = Visibility.Visible;
            }
            else
            {
                lbResult.Content = $"No words found";
                dgWords.Visibility = Visibility.Hidden;
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
                    searchResultsPopup.IsOpen = true;
                    btnClearSearch.Visibility = Visibility.Visible;

                    SuggestWords();
                }
                else
                {
                    btnClearSearch.Visibility = Visibility.Hidden;
                    searchResultsPopup.IsOpen = false;

                    //LoadAllWords();
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Cannot search");
            }
        }

        public void SearchWordByAlphabet()
        {
            lbResult.Visibility = Visibility.Visible;
            btnAll.Background = Brushes.BlueViolet;

            List<Word> words = GetResult();
            if (words.Count() > 0)
            {
                lbResult.Content = $"Found {words.Count()} words";
                dgWords.ItemsSource = words;
                dgWords.Visibility = Visibility.Visible;
            }
            else
            {
                lbResult.Content = $"No words found";
                dgWords.Visibility = Visibility.Hidden;
                vocabulary.Visibility = Visibility.Hidden;

            }
        }

        public void SuggestWords()
        {
            List<Word> words = GetResult();
            if (words.Count() > 0)
            {
                searchResultsListBox.ItemsSource = words;
                searchResultsListBox.DisplayMemberPath = "DisplayWord";
                searchResultsListBox.SelectedValuePath = "WordId";
            }
            else
            {
                searchResultsListBox.ItemsSource = null;
            }
        }

        public List<Word> GetResult()
        {
            if (String.IsNullOrEmpty(filterStart) == false)
            {
                return defaultWords.Where(w => ((w.WordName.ToUpper().StartsWith(filterStart) && w.WordName.Contains(filterSearch))
            || (filterSearch.Contains(w.WordName) && w.WordName.ToUpper().StartsWith(filterStart)))).ToList();
            }
            else
            {
                return defaultWords.Where(w => w.WordName.StartsWith(filterSearch)).ToList();
            }
        }

        public void AssignDefaultWords()
        {
            using (var context = new DictionaryContext())
            {
                defaultWords = context.Words.Include(w => w.Type).ToList();
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
                tb.MaxWidth = ScreenWidth;
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
                tb.MaxWidth = ScreenWidth;

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
            SearchWordByAlphabet();

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

        private void searchResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
