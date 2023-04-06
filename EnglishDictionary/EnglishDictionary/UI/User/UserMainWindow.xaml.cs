using EnglishDictionary.Models;
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
        }

        private int _targetWordId;
        private string filterSearch = "";
        private StackPanel sp;
        private TextBlock tb;


        private void lvResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = (ListView)sender;

            Word word = lv.SelectedItem as Word;
            if (word != null)
            {
                _targetWordId = word.WordId;

                LoadWordDetails();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            filterSearch = txt.Text;
            if (String.IsNullOrEmpty(txt.Text) == false)
            {
                List<Word> words = GetResult();
                if (words.Count() > 0)
                {
                    lvResults.ItemsSource = words;
                    resultContainer.Visibility = Visibility.Visible;
                }
                else
                {
                    resultContainer.Visibility = Visibility.Hidden;
                    vocabulary.Visibility = Visibility.Hidden;
                }
            } else
            {
                resultContainer.Visibility = Visibility.Hidden;
                vocabulary.Visibility = Visibility.Hidden;

            }
        }

        public List<Word> GetResult()
        {
            using (var context = new DictionaryContext())
            {
                return context.Words.Include(w => w.Type).Where(w => w.WordName.Contains(filterSearch) 
                || filterSearch.Contains(w.WordName)).ToList();
            }
        }

        private void LoadWordDetails()
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Hide();

            MainWindowView window = new MainWindowView();


            Application.Current.MainWindow = window;
            Application.Current.MainWindow.Show();
        }

        private void lvResults_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
