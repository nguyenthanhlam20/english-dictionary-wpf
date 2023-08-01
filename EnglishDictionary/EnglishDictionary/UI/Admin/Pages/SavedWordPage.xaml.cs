using EnglishDictionary.Models;
using FinancialWPFApp.UI;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace EnglishDictionary.UI.Admin.Pages
{
    /// <summary>
    /// Interaction logic for SavedWordPage.xaml
    /// </summary>
    public partial class SavedWordPage : Page, INotifyPropertyChanged
    {


        private int totalRecord = 0;
        private int pageSize = 10;
        private int totalPage = 0;
        private int currentPage = 1;
        private string filterSearch = "";
        private bool _allowChangePage = true;

        private List<Word> words = new();

        private AddWordWindow _addWindow;
        private ViewWordWindow _viewWindow;
        private EditWordWindow _editWindow;

        public ReplayCommand EditWordCommand { get; set; }
        public ReplayCommand ViewWordCommand { get; set; }
        public ReplayCommand DeleteWordCommand { get; set; }
        public ReplayCommand SelectWordCommand { get; set; }
        public ReplayCommand SaveWordCommand { get; set; }

        private double _screenHeight = 0f;
        public double ScreenHeight
        {
            get { return _screenHeight; }
            set
            {
                _screenHeight = value;
                OnPropertyChanged("ScreenHeight");
            }
        }

        public SavedWordPage()
        {
            ScreenHeight = SystemParameters.PrimaryScreenHeight - 300;

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
            EditWordCommand = new ReplayCommand(ShowEditWordWindow);
            ViewWordCommand = new ReplayCommand(ShowViewWordWindow);
            DeleteWordCommand = new ReplayCommand(ShowConfirmWindow);
            SelectWordCommand = new ReplayCommand(SelectWord);
            SaveWordCommand = new ReplayCommand(SaveWord);
        }


        public void SaveWord(object parameter)
        {
            try
            {
                using (var context = new DictionaryContext())
                {
                    int wordId = (int)parameter;

                    Word word = words.SingleOrDefault(w => w.WordId == wordId);
                    if (word.IsAdminSaved == false)
                    {
                        word.IsAdminSaved = true;
                        word.IconName = "ContentSaveCheck";
                    }
                    else
                    {
                        word.IsAdminSaved = false;
                        word.IconName = "ContentSaveOff";
                    }
                    context.Words.Update(word);

                    context.SaveChanges();

                    words = GetWords();

                    if (words.Count() == 0)
                    {
                        LoadWordInitialization();
                    }
                    else
                    {

                        dgWords.ItemsSource = new List<Word>(words);
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
                btnUnsaveAll.Visibility = Visibility.Visible;
            }
            else
            {
                btnDeleteAll.Visibility = Visibility.Hidden;
                btnUnsaveAll.Visibility = Visibility.Hidden;

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
            _editWindow = new EditWordWindow(wordId, null, this);
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
            try
            {
                IsSelectAllRecord = false;
                words = GetWords();
                InitializePagination(words);
                dgWords.ItemsSource = words;
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

                string msg = "";

                List<Word> words = context.Words.Include(w => w.Type).Where(w => (w.WordName.Contains(filterSearch)
                || w.Type.WordTypeName.Contains(filterSearch)) && w.IsAdminSaved == true).ToList();

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

        private void btnAddWord_Click(object sender, RoutedEventArgs e)
        {

            _addWindow = new AddWordWindow(null, this);
            _addWindow.Show();
        }


        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete all selected words?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
                        btnUnsaveAll.Visibility = Visibility.Hidden;
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
            btnUnsaveAll.Visibility = Visibility.Visible;
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
            btnUnsaveAll.Visibility = Visibility.Hidden;

            dgWords.ItemsSource = words;

        }

        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
        }

        private void UnSaveWord(int _wordId)
        {
            using (var context = new DictionaryContext())
            {
                Word word = context.Words.SingleOrDefault(w => w.WordId == _wordId);
                word.IsSelected = false;

                word.IsAdminSaved = false;
                word.IconName = "ContentSaveOff";

                context.Words.Update(word);
                context.SaveChanges();
            }
        }


        private void btnUnsaveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = 0;

                MessageBoxResult result = MessageBox.Show("Are you sure you want to unsave all selected words?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (Word word in words)
                    {
                        if (word.IsSelected == true)
                        {
                            count++;
                            UnSaveWord(word.WordId);
                        }
                    }
                    if (count > 0)
                    {

                        IsSelectAllRecord = false;
                        LoadWordInitialization();
                        btnUnsaveAll.Visibility = Visibility.Hidden;
                        btnDeleteAll.Visibility= Visibility.Hidden;
                        MessageBox.Show($"Unsave {count} words successful");
                    }
                }
            }
            catch (Exception)
            {


            }
        }

        public void ResizeTable(double actualHeight)
        {
            ScreenHeight = actualHeight - 250;

            dgWords.MaxHeight = ScreenHeight;
        }
    }
}
