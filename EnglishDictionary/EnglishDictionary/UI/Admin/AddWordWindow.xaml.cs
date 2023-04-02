using EnglishDictionary.Models;
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
    /// Interaction logic for AddWordWindow.xaml
    /// </summary>
    public partial class AddWordWindow : Window
    {

        private Button btnRemove;
        private StackPanel spConatainer;
        private TextBox txtInput;
        private int reusableInt = 0;


        private AdminMainWindow _mainWindow;

        public AddWordWindow(AdminMainWindow mainWindow)
        {
            InitializeComponent();
            LoadWordTypes();

            _mainWindow = mainWindow;
        }

        public void LoadWordTypes()
        {
            using (var context = new DictionaryContext())
            {
                cbWordType.ItemsSource = context.WordTypes.ToList();
                cbWordType.DisplayMemberPath = "WordTypeName";
                cbWordType.SelectedValuePath = "WordTypeId";
                cbWordType.SelectedIndex = 0;
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

        public void CreateInputText(string name, string tag)
        {
            txtInput = new TextBox();
            txtInput.Tag = tag;
            txtInput.Width = 680;
            txtInput.Name = name;
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

        private void btnAddMeaning_Click(object sender, RoutedEventArgs e)
        {
            // Get number of rows
            reusableInt = listMeaning.Children.Count;

            // Create container
            CreateSpContainer("spMeaning" + reusableInt);

            // Add input feild
            CreateInputText("txtMeaning" + reusableInt, "Enter new definition");

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

        private void btnAddExample_Click(object sender, RoutedEventArgs e)
        {
            // Get number of rows
            reusableInt = listExample.Children.Count;

            // Create container
            CreateSpContainer("spExample" + reusableInt);

            // Add input feild
            CreateInputText("txtExample" + reusableInt, "Enter new example");

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

        public Tuple<bool, string> ValidateInput()
        {

            // Check whether user input all required fields or not
            if (String.IsNullOrEmpty(txtWord.Text) || String.IsNullOrEmpty(txtIPA.Text))
            {
                return new Tuple<bool, string>(false, "Please enter all word name and ipa");
            }

            return new Tuple<bool, string>(true, "Valid input"); ;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            // Get validation result
            Tuple<bool, string> validationResult = ValidateInput();

            // If user pass all the criterias then allow to add new word
            if (validationResult.Item1)
            {
                AddNewWord();
                AddExamples();
                AddMeanings();
                _mainWindow.LoadWords(true);
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
                Word newWord = context.Words.OrderBy(w => w.WordId).Last();

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
                        wd = new WordMeaning() { MeaningContent = txt.Text, WordId = newWord.WordId };


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
                Word newWord = context.Words.OrderBy(w => w.WordId).Last();

                TextBox txt = new TextBox();
                WordExample wd = new WordExample();

                foreach (StackPanel sp in listExample.Children)
                {
                    txt = (TextBox)sp.Children[0];

                    // Check if the row is not empty then add
                    if (String.IsNullOrEmpty(txt.Text) == false)
                    {
                        wd = new WordExample() { ExampleContent = txt.Text, WordId = newWord.WordId };

                        context.WordExamples.Add(wd);
                    }
                }

                if (context.SaveChanges() > 0)
                {
                    //MessageBox.Show("Add examples successful");
                }
            }
        }

        private void AddNewWord()
        {
            using (var context = new DictionaryContext())
            {
                Word w = new Word()
                {

                    WordName = txtWord.Text,
                    IPA = txtIPA.Text,
                    WordTypeId = int.Parse(cbWordType.SelectedValue.ToString()),

                };
                context.Words.Add(w);
                if (context.SaveChanges() > 0)
                {
                    MessageBox.Show("Add word successful");
                }

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }


    }
}
