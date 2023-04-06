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


        private AddWordWindow _addWindow;
        private ViewWordWindow _viewWindow;
        private EditWordWindow _editWindow;

        public ReplayCommand EditWordCommand { get; set; }
        public ReplayCommand ViewWordCommand { get; set; }
        public ReplayCommand DeleteWordCommand { get; set; }
        public ReplayCommand SelectWordCommand { get; set; }


        private int totalRecord = 0;
        private int pageSize = 10;
        private int totalPage = 0;
        private int currentPage = 1;
        private string filterSearch = "";

        public List<Word> words = new();

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
            SelectWordCommand = new ReplayCommand(SelectWord);
        }

        public void SelectWord(object parameter)
        {
            int wordId = (int)parameter;

            Word word = words.SingleOrDefault(w => w.WordId == wordId);
            if (word.IsSelected == false)
            {
                words.SingleOrDefault(w => w.WordId == wordId).IsSelected = true;
            }
            else
            {
                words.SingleOrDefault(w => w.WordId == wordId).IsSelected = false;
            }

            if (words.Where(w => w.IsSelected == true).Count() > 0)
            {
                btnDeleteAll.Visibility = Visibility.Visible;
            }
            else
            {
                btnDeleteAll.Visibility = Visibility.Hidden;

            }

            dgWords.ItemsSource = words;


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
        public void DeleteWordNoConfirm(int wordId)
        {
            using (var context = new DictionaryContext())
            {
                Word word = context.Words.SingleOrDefault(w => w.WordId == wordId);
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
            words = GetWords();
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
            try
            {
                Button btn = sender as Button;

                if (btn != null)
                {
                    int pageIndex = int.Parse(btn.Content.ToString());
                    //MessageBox.Show("Clic " + pageIndex);

                    currentPage = pageIndex;
                    LoadWords();
                }
            }
            catch (Exception)
            {

            }
        }

        private void cbPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (String.IsNullOrEmpty(cb.Text) == false)
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
                int wordSuccess = 0;
                // Load the CSV file into a list of objects using CsvHelper
                string filePath = openFileDialog.FileName;
                if (String.IsNullOrEmpty(filePath) == false)
                {

                    using (var context = new DictionaryContext())
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fileStream))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true,
                        HeaderValidated = null,
                        MissingFieldFound = null,
                        IgnoreBlankLines = true,

                    }))
                    {

                        csv.Read(); // Skip the header row


                        while (csv.Read())
                        {
                            // Read the data rows
                            ImportWordData record = new ImportWordData()
                            {
                                Word = csv.GetField(0).ToString(),
                                Type = csv.GetField(1).ToString(),
                                IPA = csv.GetField(2).ToString(),
                                Definition = csv.GetField(3).ToString(),
                                Example = csv.GetField(4).ToString(),
                            };
                            if (String.IsNullOrEmpty(record.Word) == false && String.IsNullOrEmpty(record.Type) == false
                                 && String.IsNullOrEmpty(record.IPA) == false && String.IsNullOrEmpty(record.Definition) == false
                                && String.IsNullOrEmpty(record.Example) == false)
                            {


                                // Process each value
                                WordType type = context.WordTypes.SingleOrDefault(w => w.WordTypeName == record.Type.Trim());
                                if (type == null)
                                {
                                    type = context.WordTypes.SingleOrDefault(w => w.WordTypeName == "noun");
                                }
                                Word word = new Word()
                                {
                                    WordName = record.Word.Trim(),
                                    WordTypeId = type.WordTypeId,
                                    IPA = record.IPA.Trim(),
                                };

                                context.Words.Add(word);
                                if (context.SaveChanges() > 0)
                                {
                                    wordSuccess++;
                                }

                                Word newWord = context.Words.OrderBy(w => w.WordId).Last();

                                string[] meanings = record.Definition.Split("\n");


                                foreach (var meaning in meanings)
                                {
                                    if (String.IsNullOrEmpty(meaning) == false)
                                    {
                                        WordMeaning me = new WordMeaning()
                                        {
                                            WordId = newWord.WordId,
                                            MeaningContent = meaning,
                                        };

                                        context.WordMeanings.Add(me);
                                    }
                                }

                                string[] examples = record.Example.Split("\n");
                                foreach (var example in examples)
                                {
                                    if (String.IsNullOrEmpty(example) == false)
                                    {
                                        WordExample ex = new WordExample()
                                        {
                                            WordId = newWord.WordId,
                                            ExampleContent = example,
                                        };
                                        context.WordExamples.Add(ex);
                                    }
                                }

                            }
                        }



                        int newWords = context.SaveChanges();

                        if (newWords > 0)
                        {
                            MessageBox.Show($"Import {wordSuccess} words successful");
                            LoadWordInitialization();

                        }
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Cannot import file, error " + ex);
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
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";

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
                    using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                    using (var writer = new StreamWriter(stream, encoding))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        // Write the header row
                        csv.WriteField("Word");
                        csv.WriteField("Type");
                        csv.WriteField("IPA");
                        csv.WriteField("Definition");
                        csv.WriteField("Example");
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
                                csv.WriteField(meaningStr);
                                csv.WriteField(exampleStr);
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

                MessageBox.Show("Please close the file you want to override first");
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

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete all selected words?", "Question", MessageBoxButton.YesNo);

                int count = 0;
                if (result == MessageBoxResult.Yes)
                {
                    foreach (Word word in words)
                    {
                        if (word.IsSelected == true)
                        {
                            count++;
                            DeleteWordNoConfirm(word.WordId);
                        }
                    }
                    if (count > 0)
                    {
                        MessageBox.Show($"Delete {count} words successful");
                        LoadWordInitialization();
                        btnDeleteAll.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Cannot delete all selected words");
            }
        }

        private void cbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < words.Count(); i++)
            {
                words[i].IsSelected = true;
            }

            btnDeleteAll.Visibility = Visibility.Visible;
            dgWords.ItemsSource = words;
        }

        private void cbSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < words.Count(); i++)
            {
                words[i].IsSelected = false;
            }
            btnDeleteAll.Visibility = Visibility.Hidden;
          
            dgWords.ItemsSource = words;

        }
    }
}
