using FinancialWPFApp.UI.Public.ViewModels.Pages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace FinancialWPFApp.UI.Public.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : System.Windows.Controls.Page
    {

        private LoginViewModel _viewModel;
        private string password = "";

        private bool isShowPassword = false;
        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
        }

        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox txt = sender as TextBox;

            if (password.Length < txt.Text.Length)
            {
                if(txt.Text.ElementAt(txt.Text.Length - 1).ToString() != "*")
                {
                    password += txt.Text.ElementAt(txt.Text.Length - 1);
              }
               

            }
            if (password.Length > txt.Text.Length)
            {
                if (password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                }
            }

            if (isShowPassword == false)
            {
                string maskedString = new string('*', txt.Text.Length);
                if (maskedString.Length > 0)
                {

                    txt.Text = txt.Text.Replace(txt.Text, maskedString);
                }
            }

          
          
            //MessageBox.Show(password);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = password;
        }
    }
}
