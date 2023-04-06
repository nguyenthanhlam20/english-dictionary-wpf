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

namespace EnglishDictionary.UI.Admin
{
    /// <summary>
    /// Interaction logic for EditWordWindow.xaml
    /// </summary>
    public partial class EditWordWindow : Window
    {
        private Button btnRemove;
        private StackPanel spConatainer;
        private TextBox txtInput;
        private int reusableInt = 0;


        private AdminMainWindow _mainWindow;
        private int _wordId;

        public EditWordWindow(int wordId, AdminMainWindow mainWindow)
        {
            InitializeComponent();
            _wordId = wordId;
            _mainWindow = mainWindow;

            LoadWordTypes();
            LoadWordDetails();
        }

        public void LoadWordDetails()
        {
            using (var context = new DictionaryContext())
            {

                // Set word general details
                Word word = context.Words.Include(w => w.Type).SingleOrDefault(w => w.WordId == _wordId);
                SetGeneralDetail(word);

                // Set word examples
                List<WordExample> examples = context.WordExamples.Where(w => w.WordId == _wordId).ToList();
                SetExamples(examples);

                // Set word examples
                List<WordMeaning> meanings = context.WordMeanings.Where(w => w.WordId == _wordId).ToList();
                SetMeanings(meanings);

            }
        }

        public void SetGeneralDetail(Word word)
        {
            txtWord.Text = word.WordName;
            txtIPA.Text = word.IPA;
            cbWordType.SelectedValue = word.Type.WordTypeId;
        }

        public void SetExamples(List<WordExample> examples)
        {
            listExample.Children.Clear();

            for (int i = 0; i < examples.Count(); i++)
            {
               
                WordExample we = examples[i];
                // Get number of rows
          
                // Create container
                CreateSpContainer("spExample" + i);

               

                if (i != 0)
                {
                    // Add input feild
                    CreateInputText("txtExample" + i, "Enter new example", we.ExampleContent);
                    // Add remove button
                    CreateRemoveBtn("btnExample" + i);
                    btnRemove.Click += BtnRemoveExample_Click;
                    spConatainer.Children.Add(txtInput);
                    spConatainer.Children.Add(btnRemove);
                    // Add textbox and button to stack panel
                } else
                {
                    // Add input feild
                    CreateInputText("txtExample" + i, "Enter example", we.ExampleContent);
                    // Add textbox and button to stack panel
                    spConatainer.Children.Add(txtInput);
                }


              

                // Register name for container
                listExample.RegisterName(spConatainer.Name, spConatainer);

                // Add to list meaning
                listExample.Children.Add(spConatainer);
            }
        }

        public void SetMeanings(List<WordMeaning> meanings)
        {
            listMeaning.Children.Clear();
            for (int i = 0; i < meanings.Count(); i++)
            {
                WordMeaning we = meanings[i];
                // Create container
                CreateSpContainer("spMeaning" + i);

                if (i != 0)
                {
                    // Add input feild
                    CreateInputText("txtMeaning" + i, "Enter new definition", we.MeaningContent);
                    // Add remove button
                    CreateRemoveBtn("btnMeaning" + i);
                    btnRemove.Click += BtnRemoveMeaning_Click;
                    // Add textbox and button to stack panel
                    spConatainer.Children.Add(txtInput);
                    spConatainer.Children.Add(btnRemove);
                }
                else
                {
                    // Add textbox and button to stack panel
                    CreateInputText("txtMeaning" + i, "Enter definition", we.MeaningContent);
                    spConatainer.Children.Add(txtInput);
                    // Add input feild
                }


              

                // Register name for container
                listMeaning.RegisterName(spConatainer.Name, spConatainer);

                // Add to list meaning
                listMeaning.Children.Add(spConatainer);
            }
        }

        public void CreateSpContainer(string name)
        {
            // Stack Panel Container
            spConatainer = new StackPanel();
            spConatainer.Name = name;
            spConatainer.Orientation = Orientation.Horizontal;
            spConatainer.Margin = new Thickness(0, 10, 0, 0);
        }

        public void CreateInputText(string name, string tag, string value)
        {
            txtInput = new TextBox();
            txtInput.Tag = tag;
            txtInput.Width = 680;
            txtInput.Name = name;
            txtInput.Text = value;
            txtInput.Style = System.Windows.Application.Current.Resources["StyledTextBox"] as System.Windows.Style;
        }

        public void CreateRemoveBtn(string name)
        {
            btnRemove = new Button();
            btnRemove.Name = name;
            btnRemove.Content = "Remove";
            btnRemove.Width = 80;
            btnRemove.Height = 30;

            btnRemove.Margin = new Thickness(10, 0, 0, 0);
            btnRemove.Style = System.Windows.Application.Current.Resources["PagingButton"] as System.Windows.Style;
            btnRemove.Background = System.Windows.Application.Current.Resources["ButtonHover"] as Brush;
            btnRemove.Foreground = System.Windows.Application.Current.Resources["TertiaryWhiteColor"] as Brush;
        }
        private void btnAddExample_Click(object sender, RoutedEventArgs e)
        {
            // Get number of rows
            reusableInt = listExample.Children.Count;

            // Create container
            CreateSpContainer("spExample" + reusableInt);

            // Add input feild
            CreateInputText("txtExample" + reusableInt, "Enter new example", "");

            // Add remove button
            CreateRemoveBtn("btnExample" + reusableInt);
            btnRemove.Click += BtnRemoveExample_Click;


            // Add textbox and button to stack panel
            spConatainer.Children.Add(txtInput);
            spConatainer.Children.Add(btnRemove);

            // Register name for container
            listExample.RegisterName(spConatainer.Name, spConatainer);

            // Add to list meaning
            listExample.Children.Add(spConatainer);
        }


        private void btnAddMeaning_Click(object sender, RoutedEventArgs e)
        {
            // Get number of rows
            reusableInt = listMeaning.Children.Count;

            // Create container
            CreateSpContainer("spMeaning" + reusableInt);

            // Add input feild
            CreateInputText("txtMeaning" + reusableInt, "Enter new definition", "");

            // Add remove button
            CreateRemoveBtn("btnMeaning" + reusableInt);
            btnRemove.Click += BtnRemoveMeaning_Click;


            // Add textbox and button to stack panel
            spConatainer.Children.Add(txtInput);
            spConatainer.Children.Add(btnRemove);

            // Register name for container
            listMeaning.RegisterName(spConatainer.Name, spConatainer);

            // Add to list meaning
            listMeaning.Children.Add(spConatainer);
        }


        private void BtnRemoveMeaning_Click(object sender, RoutedEventArgs e)
        {
            // Get index
            Button btn = sender as Button;
            reusableInt = int.Parse(btn.Name.Replace("btnMeaning", ""));

            // Find container
            StackPanel sp = (StackPanel)listMeaning.FindName("spMeaning" + reusableInt);

            // Remove container
            listMeaning.UnregisterName(sp.Name);
            listMeaning.Children.Remove(sp);
        }

        private void BtnRemoveExample_Click(object sender, RoutedEventArgs e)
        {
            // Get index
            Button btn = sender as Button;
            reusableInt = int.Parse(btn.Name.Replace("btnExample", ""));

            // Find container
            StackPanel sp = (StackPanel)listExample.FindName("spExample" + reusableInt);

            // Remove container
            listExample.UnregisterName(sp.Name);
            listExample.Children.Remove(sp);
        }



        public void LoadWordTypes()
        {
            using (var context = new DictionaryContext())
            {
                cbWordType.ItemsSource = context.WordTypes.ToList();
                cbWordType.DisplayMemberPath = "WordTypeName";
                cbWordType.SelectedValuePath = "WordTypeId";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        public Tuple<bool, string> ValidateInput()
        {

            // Check whether user input all required fields or not
            if (String.IsNullOrEmpty(txtWord.Text) || String.IsNullOrEmpty(txtIPA.Text))
            {
                return new Tuple<bool, string>(false, "Please enter all word name and ipa");
            }

            return new Tuple<bool, string>(true, "Valid input"); ;
        }

        public void DeleteExampleAndMeaning(int wordId)
        {
            using (var context = new DictionaryContext())
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

                if (context.SaveChanges() > 0)
                {
                   

                }

            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DeleteExampleAndMeaning(_wordId);
            // Get validation result
            Tuple<bool, string> validationResult = ValidateInput();

            // If user pass all the criterias then allow to add new word
            if (validationResult.Item1)
            {
                UpdateWord();
                AddMeanings();
                AddExamples();
                _mainWindow.LoadWords();
                this.Close();
            }
            else
            {
                MessageBox.Show(validationResult.Item2);
            }
        }



        public void AddExamples()
        {
            using (var context = new DictionaryContext())
            {
               
                TextBox txt = new TextBox();
                WordMeaning wd = new WordMeaning();

                // Interate through each text box in stack panel
                foreach (StackPanel sp in listMeaning.Children)
                {
                    // Get textbox of each row in stack panel
                    txt = (TextBox)sp.Children[0];

                    // Check whether this text box is null or not if not so then add
                    if (!String.IsNullOrEmpty(txt.Text))
                    {

                        // Create new Word Meaning
                        wd = new WordMeaning() { MeaningContent = txt.Text, WordId = _wordId };


                        // Add it to database
                        context.WordMeanings.Add(wd);
                    }
                }

                if (context.SaveChanges() > 0)
                {
                    //MessageBox.Show("Add meanings successful");
                }
            }
        }

        public void AddMeanings()
        {
            using (var context = new DictionaryContext())
            {

                TextBox txt = new TextBox();
                WordExample wd = new WordExample();

                foreach (StackPanel sp in listExample.Children)
                {
                    txt = (TextBox)sp.Children[0];

                    // Check if the row is not empty then add
                    if (String.IsNullOrEmpty(txt.Text) == false)
                    {
                        wd = new WordExample() { ExampleContent = txt.Text, WordId = _wordId };

                        context.WordExamples.Add(wd);
                    }
                }

                if (context.SaveChanges() > 0)
                {
                    //MessageBox.Show("Add examples successful");
                }
            }
        }

        private void UpdateWord()
        {
            using (var context = new DictionaryContext())
            {
                Word w = new Word()
                {
                    WordId = _wordId,
                    WordName = txtWord.Text,
                    IPA = txtIPA.Text,
                    WordTypeId = int.Parse(cbWordType.SelectedValue.ToString()),

                };
                context.Words.Update(w);
                if (context.SaveChanges() > 0)
                {
                    MessageBox.Show("Update word successful");
                }

            }
        }
    }
}
