using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinancialWPFApp.Themes
{
   public static class ThemeController
    {
        public enum ThemeTypes
        {
            Light,
            Dark
        }

        public static ThemeTypes CurrentTheme { get; set; }

        public static ResourceDictionary ThemeDictionary
        {
            get
            {
                return Application.Current.Resources.MergedDictionaries[0];
            }
            set
            {
                Application.Current.Resources.MergedDictionaries[0] = value;
            }
        }

        private static void ChangeTheme(Uri uri)
        {
            ThemeDictionary = new ResourceDictionary() { Source = uri };
        }


        public static void SetTheme(ThemeTypes theme) 
        {
            string themeName = "";
            CurrentTheme = theme;
            switch(theme)
            {
                case ThemeTypes.Light: 
                    themeName= "LightTheme";
                    break;
                case ThemeTypes.Dark:
                    themeName = "DarkTheme";
                    break;
            }

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                {
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative)); ;
                }
                //MessageBox.Show($"Themes/{themeName}.xaml");
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error when trying to change theme: {ex}");
            }
            
        }
    }
}
