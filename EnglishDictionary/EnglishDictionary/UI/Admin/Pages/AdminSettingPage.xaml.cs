using EnglishDictionary.Models;
using EnglishDictionary.Properties;
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

namespace EnglishDictionary.UI.Admin.Pages
{
    /// <summary>
    /// Interaction logic for AdminSettingPage.xaml
    /// </summary>
    public partial class AdminSettingPage : Page
    {
        public AdminSettingPage()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string oldPassword = txtOldPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (String.IsNullOrEmpty(oldPassword)
                || String.IsNullOrEmpty(newPassword)
                || String.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please enter old password and new password");
            }
            else
            {
                if (oldPassword == Settings.Default.Password)
                {
                    if (newPassword == confirmPassword)
                    {
                        using (var context = new DictionaryContext())
                        {
                            Account acc = new Account()
                            {
                                Username = Settings.Default.Username,
                                Password = newPassword,
                                Role = Settings.Default.Role
                            };

                            context.Accounts.Update(acc);

                            if (context.SaveChanges() > 0)
                            {
                                Settings.Default.Password = newPassword;
                                Settings.Default.Save();

                                MessageBox.Show("Change password successful");
                                txtOldPassword.Clear();
                                txtConfirmPassword.Clear();
                                txtNewPassword.Clear();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("New password doesn't match");

                    }
                }
                else
                {
                    MessageBox.Show("Wrong old password");
                }

            }
        }
    }
}
