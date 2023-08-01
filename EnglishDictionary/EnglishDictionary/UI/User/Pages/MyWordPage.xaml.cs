using CsvHelper.Configuration;
using CsvHelper;
using EnglishDictionary.Models;
using EnglishDictionary.UI.Admin;
using FinancialWPFApp.UI;
using MahApps.Metro.IconPacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnglishDictionary.UI.User.Pages
{
    /// <summary>
    /// Interaction logic for MyWordPage.xaml
    /// </summary>
    public partial class MyWordPage : Page, INotifyPropertyChanged
    {

        private int totalRecord = 0;
        private int pageSize = 10;
        private int totalPage = 0;
        private int currentPage = 1;
        private string filterSearch = "";
        private bool _allowChangePage = true;

        private List<Word> words = new();

        private ViewWordWindow _viewWindow;

        public ReplayCommand EditWordCommand { get; set; }
        public ReplayCommand ViewWordCommand { get; set; }
        public ReplayCommand SelectWordCommand { get; set; }

        public ReplayCommand CleanWordCommand { get; set; }

        public MyWordPage()
        {
            ScreenHeight = SystemParameters.PrimaryScreenHeight - 400;
            InitializeComponent();
            InitializePageSize();
            InitializeCommand();

            LoadWords();
            DataContext = this;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isSelectAllRecord = false;

        public bool IsSelectAllRecord
        {
            get { return _isSelectAllRecord; }
            set
            {
                _isSelectAllRecord = value;
                OnPropertyChanged("IsSelectAllRecord");
            }
        }

        public double _screenHeight = 0;

        public double ScreenHeight
        {
            get { return _screenHeight; }
            set
            {
                _screenHeight = value;
                OnPropertyChanged("ScreenHeight");
            }
        }


        public void LoadWordInitialization()
        {
            pageSize = 10;
            currentPage = 1;
            filterSearch = "";
            txtSearch.Text = "";
            cbPage.Text = "10";
            LoadWords();
        }


        public void InitializeCommand()
        {
            ViewWordCommand = new ReplayCommand(ShowViewWordWindow);
            SelectWordCommand = new ReplayCommand(SelectWord);
            CleanWordCommand = new ReplayCommand(CleanWord);
        }


        public void CleanWord(object parameter)
        {
            try
            {
                using (var context = new DictionaryContext())
                {
                    int wordId = (int)parameter;

                    Word word = words.SingleOrDefault(w => w.WordId == wordId);
                    word.IsHistory = false;
                    word.SearchIconName = "Magnify";
                    context.Words.Update(word);

                    context.SaveChanges();

                    words = GetWords();
                    if(words.Count() > 0)
                    {
                        dgWords.ItemsSource = words;
                    } else
                    {
                        LoadWordInitialization();
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error saving word");
            }

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
        
  
        public void ShowViewWordWindow(object parameter)
        {
            int wordId = (int)parameter;
            _viewWindow = new ViewWordWindow(wordId);
            _viewWindow.Show();

        }


        public void LoadWords()
        {
            try
            {
                IsSelectAllRecord = false;
                words = GetWords();
                InitializePagination(words);
                dgWords.ItemsSource = words;

                btnEmpty.Visibility = words.Count() > 0 ? Visibility.Visible : Visibility.Hidden;


            }
            catch (Exception)
            {

                MessageBox.Show("Error when loading word to table");
            }
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

            // Calculate the number of pages to show at a time
            int maxPages = 4;

            // Calculate the range of pages to show
            int startPage = Math.Max(currentPage - (maxPages / 2), 1);
            int endPage = Math.Min(startPage + maxPages - 1, totalPage);

            // Add the page buttons
            for (int i = startPage; i <= endPage; i++)
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

            // Add a "Previous" button if necessary
            if (startPage > 1)
            {
                btn = new Button();
                btn.Style = Application.Current.Resources["PagingButton"] as Style;
                btn.Click += btnLeft_Click;

                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.ChevronLeft;

                btn.Content = icon;
                pageContainer.Children.Insert(0, btn);
            }

            // Add a "Next" button if necessary
            if (endPage < totalPage)
            {
                btn = new Button();
                btn.Style = Application.Current.Resources["PagingButton"] as Style;
                btn.Click += btnRight_Click;

                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.ChevronRight;

                btn.Content = icon;

                pageContainer.Children.Add(btn);
            }
        }


        public List<Word> GetWords()
        {

            if (String.IsNullOrEmpty(filterSearch) == false && _allowChangePage == false)
            {
                currentPage = 1;
                pageSize = 10;
                _allowChangePage = true;
            }
            if (String.IsNullOrEmpty(filterSearch) == true && _allowChangePage == true)
            {
                _allowChangePage = false;
            }
            using (var context = new DictionaryContext())
            {

                List<Word> words = context.Words.Include(w => w.Type)
                    .Where(x => x.IsHistory == true && x.WordName.Contains(filterSearch))
                    .ToList();

                string msg = "";
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

                int pageIndex = int.Parse(btn.Content.ToString());
                if (currentPage != pageIndex)
                {
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
            try
            {
                ComboBox cb = sender as ComboBox;
                if (String.IsNullOrEmpty(cb.Text) == false)
                {

                    pageSize = int.Parse(cb.SelectedValue.ToString());

                    currentPage = 1;
                    LoadWords();
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error when trying to change new page size");
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

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage != 1)
            {
                currentPage = 1;
                LoadWords();
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage != totalPage)
            {
                currentPage = totalPage;
                LoadWords();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearch.Text != null)
            {
                if (String.IsNullOrEmpty(txtSearch.Text))
                {
                    btnClearSearch.Visibility = Visibility.Hidden;
                }
                else
                {
                    btnClearSearch.Visibility = Visibility.Visible;
                }

                filterSearch = txtSearch.Text;
                LoadWords();
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
        private void btnEmpty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to empty history?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new DictionaryContext())
                    {
                        foreach (Word word in words)
                        {
                            Word currentWord = context.Words.SingleOrDefault(x => x.WordId == word.WordId);

                            currentWord.IsHistory = false;
                            currentWord.SearchIconName = "Magnify";
                            context.Update(currentWord);
                        }
                        if (context.SaveChanges() > 0)
                        {
                            MessageBox.Show($"Empty history successful");
                            LoadWordInitialization();
                            btnDeleteAll.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Cannot empty history");
            }
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove all selected words from history?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using(var context = new DictionaryContext())
                    {
                        int count = 0;
                        foreach (Word word in words)
                        {
                           if(word.IsSelected)
                            {
                                count++;
                                Word currentWord = context.Words.SingleOrDefault(x => x.WordId == word.WordId);

                                currentWord.IsHistory = false;
                                currentWord.SearchIconName = "Magnify";
                                context.Update(currentWord);
                            }
                        }
                        if (context.SaveChanges() > 0)
                        {
                            MessageBox.Show($"Remove {count} words from history successful!");
                            LoadWordInitialization();
                            btnDeleteAll.Visibility = Visibility.Hidden;
                        }
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

            words = new List<Word>(words);

            btnDeleteAll.Visibility = Visibility.Visible;
            dgWords.ItemsSource = words;
        }

        private void cbSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < words.Count(); i++)
            {
                words[i].IsSelected = false;
            }

            words = new List<Word>(words);
            btnDeleteAll.Visibility = Visibility.Hidden;

            dgWords.ItemsSource = words;

        }

        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
        }

        private void SaveWord(int _wordId)
        {
            using (var context = new DictionaryContext())
            {
                Word word = context.Words.SingleOrDefault(w => w.WordId == _wordId);
                word.IsSelected = false;
                word.IsAdminSaved = true;
                word.IconName = "ContentSaveCheck";
                context.Words.Update(word);
                context.SaveChanges();
            }
        }

        public void ResizeTable(double height)
        {
            ScreenHeight = height - 260;

            dgWords.MaxHeight = ScreenHeight;
        }

      
    }
}
