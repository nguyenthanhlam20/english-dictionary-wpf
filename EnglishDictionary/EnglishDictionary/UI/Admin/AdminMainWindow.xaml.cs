using CsvHelper;
using CsvHelper.Configuration;
using EnglishDictionary.Models;
using FinancialWPFApp.UI;
using FinancialWPFApp.UI.Public.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
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
namespace EnglishDictionary.UI.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {


        private AddWordWindow _addWindow;
        private ViewWordWindow _viewWindow;
        private EditWordWindow _editWindow;

        public ReplayCommand EditWordCommand { get; set; }
        public ReplayCommand ViewWordCommand { get; set; }
        public ReplayCommand DeleteWordCommand { get; set; }


        private int totalRecord = 0;
        private int pageSize = 10;
        private int totalPage = 0;
        private int currentPage = 1;
        private string filterSearch = "";


        public void LoadWordInitialization()
        {
            pageSize = 10;
            currentPage = 1;
            filterSearch = "";
            LoadWords();
        }

        public AdminMainWindow()
        {
            InitializeComponent();
            InitializePageSize();
            InitializeCommand();

            LoadWords();
            DataContext = this;
        }

        public void InitializeCommand()
        {
            EditWordCommand = new ReplayCommand(ShowEditWordWindow);
            ViewWordCommand = new ReplayCommand(ShowViewWordWindow);
            DeleteWordCommand = new ReplayCommand(ShowConfirmWindow);
        }

        public void ShowConfirmWindow(object parameter)
        {
            int wordId = (int)parameter;

            DeleteWord(wordId);
        }

        public void DeleteWord(int wordId)
        {
            using (var context = new DictionaryContext())
            {
                Word word = context.Words.SingleOrDefault(w => w.WordId == wordId);

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete \"{word.WordName}\" word?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    List<WordExample> examples = context.WordExamples.Where(w => w.WordId == wordId).ToList();
                    List<WordMeaning> meanings = context.WordMeanings.Where(w => w.WordId == wordId).ToList();


                    foreach (WordExample example in examples)
                    {
                        context.WordExamples.Remove(example);
                    }

                    foreach (WordMeaning meaning in meanings)
                    {
                        context.WordMeanings.Remove(meaning);
                    }

                    context.Words.Remove(word);
                    if (context.SaveChanges() > 0)
                    {
                        LoadWords();
                        MessageBox.Show("Delete word successful");

                    }
                }

            }
        }

        public void ShowEditWordWindow(object parameter)
        {
            int wordId = (int)parameter;
            _editWindow = new EditWordWindow(wordId, this);
            _editWindow.Show();

        }

        public void ShowViewWordWindow(object parameter)
        {
            int wordId = (int)parameter;
            _viewWindow = new ViewWordWindow(wordId);
            _viewWindow.Show();

        }


        public void LoadWords()
        {
            List<Word> words = GetWords();
            InitializePagination(words);
            dgWords.ItemsSource = words;
        }

        public void InitializePageSize()
        {
            cbPage.Items.Add(10);
            cbPage.Items.Add(30);
            cbPage.Items.Add(50);
            cbPage.Items.Add(100);
            cbPage.SelectedIndex = 0;
        }




        public void InitializePagination(List<Word> words)
        {
            pageContainer.Children.Clear();
            if (totalRecord == 0)
            {
                bottomContent.Visibility = Visibility.Collapsed;
                dgWords.Visibility = Visibility.Collapsed;
                lbNoRecords.Visibility = Visibility.Visible;
            }
            else
            {
                bottomContent.Visibility = Visibility.Visible;
                dgWords.Visibility = Visibility.Visible;
                lbNoRecords.Visibility = Visibility.Collapsed;
            }
            Button btn = new Button();

            //MessageBox.Show(pageSize.ToString());
            for (int i = 1; i <= totalPage; i++)
            {
                btn = new Button();
                btn.Content = i.ToString();
                btn.Style = Application.Current.Resources["PagingButton"] as Style;

                if (currentPage == i)
                {
                    btn.Background = Application.Current.Resources["ButtonHover"] as Brush;
                    btn.Foreground = Application.Current.Resources["TertiaryWhiteColor"] as Brush;

                }
                btn.Click += Btn_Click;
                pageContainer.Children.Add(btn);

            }
        }


        public List<Word> GetWords()
        {

            using (var context = new DictionaryContext())
            {

                string msg = "";

                List<Word> words = context.Words.Include(w => w.Type).Where(w => w.WordName.Contains(filterSearch)
                || w.Type.WordTypeName.Contains(filterSearch)).ToList();

                totalRecord = words.Count();

                totalPage = totalRecord % pageSize != 0 ? (totalRecord / pageSize) + 1 : totalRecord / pageSize;



                int from = (currentPage - 1) * pageSize;

                if (currentPage * pageSize >= totalRecord)
                {
                    int step = totalRecord - from;
                    words = words.GetRange(from, step);

                    msg = $"Display {(currentPage - 1) * pageSize + 1} - {totalRecord} words in total {totalRecord} words";
                }
                else
                {
                    words = words.GetRange(from, pageSize);
                    msg = $"Display {(currentPage - 1) * pageSize + 1} - {currentPage * pageSize} words in total {totalRecord} words";

                }

                lbMessage.Content = msg;

                return words;

            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            int pageIndex = int.Parse(btn.Content.ToString());
            //MessageBox.Show("Clic " + pageIndex);

            currentPage = pageIndex;
            LoadWords();
        }

        private void cbPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.Text != "")
            {
                pageSize = int.Parse(cb.SelectedValue.ToString());

                currentPage = 1;
                LoadWords();
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {

                currentPage -= 1;
                LoadWords();
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPage)
            {

                currentPage += 1;
                LoadWords();
            }
        }



        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearch.Text != null)
            {
                filterSearch = txtSearch.Text;
                LoadWords();
            }
        }

        private void btnAddWord_Click(object sender, RoutedEventArgs e)
        {
            //if (_addWindow == null )
            //{
            //    _addWindow = new AddWordWindow();
            //    _addWindow.Show();
            //}
            //else
            //{
            //    _addWindow.Activate();
            //}

            _addWindow = new AddWordWindow(this);
            _addWindow.Show();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show an open file dialog and allow the user to choose a CSV file
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                openFileDialog.Multiselect = false;
                openFileDialog.ShowDialog();

                int jump = 1;

                // Load the CSV file into a list of objects using CsvHelper
                string filePath = openFileDialog.FileName;
                if (String.IsNullOrEmpty(filePath) == false)
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        using (var context = new DictionaryContext())
                        {
                            while (csv.Read())
                            {
                                string[] row = csv.GetRecord<string[]>();

                                WordType type = context.WordTypes.SingleOrDefault(w => w.WordTypeName == row[1].Trim());
                                if (type == null)
                                {
                                    type = context.WordTypes.SingleOrDefault(w => w.WordTypeName == "noun");
                                }
                                Word word = new Word()
                                {
                                    WordName = row[0].Trim(),
                                    WordTypeId = type.WordTypeId,
                                    IPA = row[2].Trim(),
                                };

                                context.Words.Add(word);

                                Word newWord = context.Words.OrderBy(w => w.WordId).Last();

                                string[] meanings = row[3].Split("\n");

                                foreach (var meaning in meanings)
                                {
                                    WordMeaning me = new WordMeaning()
                                    {
                                        WordId = newWord.WordId + jump,
                                        MeaningContent = meaning,
                                    };

                                    context.WordMeanings.Add(me);
                                }

                                string[] examples = row[4].Split("\n");
                                foreach (var example in examples)
                                {
                                    WordExample ex = new WordExample()
                                    {
                                        WordId = newWord.WordId + jump,
                                        ExampleContent = example,
                                    };
                                    context.WordExamples.Add(ex);
                                }

                                jump++;
                            }


                            int newWords = context.SaveChanges();

                            if (newWords > 0)
                            {
                                MessageBox.Show("success " + newWords);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Cannot import file");
            }
        }
        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string templatePath = "Assets/template.csv";

                // open a stream to the template file
                Stream templateStream = Application.GetResourceStream(new Uri(templatePath, UriKind.Relative)).Stream;

                // create save file dialog
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "template.csv";
                saveFileDialog.Filter = "Excel files (*.csv)|*.csv|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    // write file contents to selected file
                    using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        templateStream.CopyTo(fileStream);
                        MessageBox.Show("Download template successful");
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Failed to download the template file");
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a SaveFileDialog to allow the user to choose the file to export to
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                saveFileDialog.Title = "Export to CSV";
                saveFileDialog.ShowDialog();
                var encoding = new System.Text.UTF8Encoding(true);
                // If the user clicked OK and entered a file name, export to the file
                if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    // Open the file for writing
                    using (var writer = new StreamWriter(saveFileDialog.FileName, false, encoding))
                    using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                    {



                        // Write the header row
                        csv.WriteField("Word".PadRight(100));
                        csv.WriteField("Type".PadRight(100));
                        csv.WriteField("IPA".PadRight(100));
                        csv.WriteField("Definition".PadRight(100));
                        csv.WriteField("Example".PadRight(100));
                        csv.NextRecord();

                        // Write the data rows
                        using (var context = new DictionaryContext())
                        {
                            List<Word> words = context.Words.Include(w => w.Type).ToList();
                            string meaningStr = "";
                            string exampleStr = "";
                            List<WordExample> examples = new();
                            List<WordMeaning> meanings = new();
                            foreach (Word word in words)
                            {
                                examples = context.WordExamples.Where(w => w.WordId == word.WordId).ToList();
                                meanings = context.WordMeanings.Where(w => w.WordId == word.WordId).ToList();


                                foreach (WordExample example in examples)
                                {
                                    exampleStr += $"{example.ExampleContent}\n";
                                }

                                foreach (WordMeaning meaning in meanings)
                                {
                                    meaningStr += $"{meaning.MeaningContent}\n";
                                }

                                csv.WriteField(word.WordName);
                                csv.WriteField(word.Type.WordTypeName);
                                csv.WriteField(word.IPA);
                                csv.WriteField(exampleStr);
                                csv.WriteField(meaningStr);
                                csv.NextRecord();


                                exampleStr = "";
                                meaningStr = "";
                            }
                        }
                    }

                    MessageBox.Show("Export to file successful. Please check path " + saveFileDialog.FileName + " to open the file");
                }
            }
            catch (Exception)
            {

                throw;
            }
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
            // Perform any cleanup operations here
            // ...

            Application.Current.Shutdown(); // Exit the application
        }
    }
}
