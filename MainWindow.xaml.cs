using Java_Bytecode_Toolkit.ExtensionsNS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Java_Bytecode_Toolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog openFileDialog = new OpenFileDialog();

        public HomeScreen homeScreen = new HomeScreen();

        public SettingsScreen settingsScreen = new SettingsScreen();

        private void OnSettingsMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.MainContentPresenter.Content = this.settingsScreen;
        }

        private void OnHomeMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.MainContentPresenter.Content = this.homeScreen;
        }

        private void OnOpenFileDialogFileOk(object sender, CancelEventArgs e)
        {
            this.homeScreen.OpenFile(this.openFileDialog.FileNames);
        }

        private void OnLightThemeMenuItemClick(object sender, RoutedEventArgs e)
        {
            App.Current.Theme = App.Current.LIGHT_THEME;
        }

        private void OnDarkThemeMenuItemClick(object sender, RoutedEventArgs e)
        {
            App.Current.Theme = App.Current.DARK_THEME;
        }

        private void OnOpenFileMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.openFileDialog.ShowDialog();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            App.Current.configuration.Save();
        }

        public MainWindow()
        {
            InitializeComponent();

            this.MainContentPresenter.Content = this.homeScreen;

            this.openFileDialog.SetFilter(
                FileDialogFilter.JAVA_CLASS_FILE_FILTER,
                FileDialogFilter.JAR_FILE_FILTER
            );

            this.OpenFileMenuItem.Click += this.OnOpenFileMenuItemClick;

            this.LightThemeMenuItem.Click += this.OnLightThemeMenuItemClick;

            this.DarkThemeMenuItem.Click += this.OnDarkThemeMenuItemClick;

            this.openFileDialog.FileOk += this.OnOpenFileDialogFileOk;

            this.HomeMenuItem.Click += this.OnHomeMenuItemClick;

            this.SettingsMenuItem.Click += this.OnSettingsMenuItemClick;

            this.Closed += this.OnClosed;
        }
    }
}
