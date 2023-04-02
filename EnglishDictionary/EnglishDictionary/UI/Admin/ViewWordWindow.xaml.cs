using EnglishDictionary.Models;
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
using System.Windows.Shapes;
using MahApps.Metro.IconPacks;

namespace EnglishDictionary.UI.Admin
{
    /// <summary>
    /// Interaction logic for ViewWordWindow.xaml
    /// </summary>
    public partial class ViewWordWindow : Window
    {


        private int _targetWordId;
        private StackPanel sp;
        private TextBlock tb;

        public ViewWordWindow(int targetWordId)
        {
            _targetWordId = targetWordId;
            InitializeComponent();

            LoadWordDetails();
        }

        private void LoadWordDetails()
        {
            using (var context = new DictionaryContext())
            {
                Word word = context.Words.Include(w => w.Type).SingleOrDefault(w => w.WordId == _targetWordId);
                GenerateWordDetails(word);

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

            txtExample.Text = examples.ElementAt(0).ExampleContent;
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
            txtMeaning.Text = meanings.ElementAt(0).MeaningContent;
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


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
