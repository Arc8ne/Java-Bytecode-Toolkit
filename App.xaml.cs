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

        public readonly string TEMP_DIR_FILE_PATH = "";

        public readonly string LOGS_DIR_FILE_PATH = "";

        public readonly string LOG_FILE_PATH = "";

        public readonly string CONFIG_DIR_FILE_PATH = "";

        public readonly string CONFIG_FILE_PATH = "";

        public readonly string OFFICIAL_GITHUB_REPO_LINK = "https://github.com/ArcaneDegree/Java-Bytecode-Toolkit";

        public readonly Logger logger = null;

        public readonly Configuration configuration = null;

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
            if (Directory.Exists(this.TEMP_DIR_FILE_PATH) == false)
            {
                Directory.CreateDirectory(this.TEMP_DIR_FILE_PATH);
            }
        }

        public App()
        {
            this.TEMP_DIR_FILE_PATH = this.BaseDirectory + "/Temp";

            this.LOGS_DIR_FILE_PATH = this.BaseDirectory + "/Logs";

            this.LOG_FILE_PATH = this.LOGS_DIR_FILE_PATH + "/log.txt";

            this.CONFIG_DIR_FILE_PATH = this.BaseDirectory + "/Config";

            this.CONFIG_FILE_PATH = this.CONFIG_DIR_FILE_PATH + "/config.json";

            this.CreateTempDirIfDoesNotExist();

            this.LIGHT_THEME = new ResourceDictionary()
            {
                Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative)
            };

            this.DARK_THEME = new ResourceDictionary()
            {
                Source = new Uri("/Themes/DarkTheme.xaml", UriKind.Relative)
            };

            this.configuration = Configuration.Open();

            this.logger = new Logger();
        }
    }
}
