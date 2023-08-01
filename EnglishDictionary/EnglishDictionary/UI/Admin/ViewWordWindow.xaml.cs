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
            try
            {
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

                MessageBox.Show("Error when trying to load word details");
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
                sp.Orientation = Orientation.Vertical;
                sp.Margin = new Thickness(0, 0, 0, 10);
               

                // Create text block for containing text
                tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 14;
                tb.Text = "- " + we.ExampleContent;
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
                sp.Orientation = Orientation.Vertical;
                sp.Margin = new Thickness(0, 0, 0, 10);

               

                // Create text block for containing text
                tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 14;
                tb.Text = "- " + we.MeaningContent;

                // Add icon and text block to row container
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
