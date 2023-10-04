using Java_Bytecode_Toolkit.ExtensionsNS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Java_Bytecode_Toolkit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly ResourceDictionary LIGHT_THEME = null;

        public readonly ResourceDictionary DARK_THEME = null;

        public readonly string tempDirFilePath = "";

        public static new App Current
        {
            get
            {
                return (App)Application.Current;
            }
        }

        public ResourceDictionary Theme
        {
            get
            {
                return this.Resources.MergedDictionaries.FirstOrDefault();
            }
            set
            {
                this.Resources.MergedDictionaries.Clear();

                this.Resources.MergedDictionaries.Add(value);
            }
        }

        public string BaseDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public new MainWindow MainWindow
        {
            get
            {
                return (MainWindow)Application.Current.MainWindow;
            }
            set
            {
                Application.Current.MainWindow = value;
            }
        }

        private void CreateTempDirIfDoesNotExist()
        {
            if (Directory.Exists(this.tempDirFilePath) == false)
            {
                Directory.CreateDirectory(this.tempDirFilePath);
            }
        }

        public App()
        {
            this.tempDirFilePath = this.BaseDirectory + "/Temp";

            this.CreateTempDirIfDoesNotExist();

            this.LIGHT_THEME = new ResourceDictionary()
            {
                Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative)
            };

            this.DARK_THEME = new ResourceDictionary()
            {
                Source = new Uri("/Themes/DarkTheme.xaml", UriKind.Relative)
            };
        }
    }
}
