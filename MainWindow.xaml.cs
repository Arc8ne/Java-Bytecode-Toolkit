using ICSharpCode.SharpZipLib.Zip;
using Java_Bytecode_Toolkit.ExtensionsNS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public const string LOADING_POPUP_DEFAULT_TEXT = "Opening file(s). Please wait...";

        public const string LOADING_POPUP_CLEANUP_TEXT = "Performing mandatory cleanup. Please wait...";

        public const string LOADING_POPUP_CANCELLATION_TEXT = "Cancelling...";

        private OpenFileDialog openFileDialog = new OpenFileDialog();

        private BackgroundWorker initBackgroundWorker = new BackgroundWorker();

        private OpenFileBackgroundWorker openFileBackgroundWorker = new OpenFileBackgroundWorker();

        public LoadingScreen loadingScreen = new LoadingScreen();

        public HomeScreen homeScreen = new HomeScreen();

        public SettingsScreen settingsScreen = new SettingsScreen();

        public AboutScreen aboutScreen = new AboutScreen();

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
            this.OpenFileLoadingPopupTextBlock.Text = LOADING_POPUP_DEFAULT_TEXT;

            this.OpenFileLoadingPopup.Visibility = Visibility.Visible;

            this.openFileBackgroundWorker.RunWorkerAsync(
                OpenFileBackgroundWorker.Task.OpenFile,
                this.openFileDialog.FileNames
            );
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

        private void OnGithubButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start(App.Current.OFFICIAL_GITHUB_REPO_LINK);
        }

        private void OnAboutMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.MainContentPresenter.Content = this.aboutScreen;
        }

        private void OnInitBackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.MainContentPresenter.Content = this.homeScreen;

            this.Toolbar.Visibility = Visibility.Visible;
        }

        private void OnInitBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                App.Current.Init();
            });

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

            this.GithubButton.Click += this.OnGithubButtonClick;

            this.AboutMenuItem.Click += this.OnAboutMenuItemClick;

            this.OpenFileLoadingPopupCancelButton.Click += this.OnOpenFileLoadingPopupCancelButtonClick;
        }

        private void OnOpenFileLoadingPopupCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.OpenFileLoadingPopupTextBlock.Text = LOADING_POPUP_CANCELLATION_TEXT;

            this.openFileBackgroundWorker.CancelAsync();
        }

        public MainWindow()
        {
            InitializeComponent();

            this.Toolbar.Visibility = Visibility.Collapsed;

            this.MainContentPresenter.Content = this.loadingScreen;

            this.initBackgroundWorker.DoWork += this.OnInitBackgroundWorkerDoWork;

            this.initBackgroundWorker.RunWorkerCompleted += this.OnInitBackgroundWorkerRunWorkerCompleted;

            this.initBackgroundWorker.RunWorkerAsync();
        }

        private class OpenFileBackgroundWorker : BackgroundWorker
        {
            private FastZip fastZip = new FastZip();

            private List<string> filePathsOfJarContentsTempDirsToClear = new List<string>();

            private string[] filePaths = new string[0];

            private Thread thread = null;

            private Task task = Task.OpenFile;

            private string GetJarFileNameFromJarFilePath(string jarFilePath)
            {
                string[] jarFilePathParts = jarFilePath.Split(
                    System.IO.Path.DirectorySeparatorChar
                );

                return jarFilePathParts[jarFilePathParts.Length - 1].Replace(".jar", "");
            }

            private void OpenJavaClassFile(string javaClassFilePath, JavaFileTreeViewItem jarFileTreeViewItem = null)
            {
                JavaClassFile javaClassFile = new JavaClassFile(javaClassFilePath);

                JavaFileTreeViewItem javaClassFileTreeViewItem = new JavaFileTreeViewItem()
                {
                    DataContext = javaClassFile
                };

                if (javaClassFileTreeViewItem == null)
                {
                    App.Current.MainWindow.homeScreen.MainTreeView.Items.Add(
                        javaClassFileTreeViewItem
                    );
                }
                else
                {
                    jarFileTreeViewItem.Items.Add(javaClassFileTreeViewItem);
                }

                App.Current.MainWindow.homeScreen.javaClassFileTreeViewItemToJavaClassFileMap[javaClassFileTreeViewItem] = javaClassFile;

                javaClassFileTreeViewItem.IsSelected = true;
            }

            private void OpenJarFile(string jarFilePath)
            {
                string jarContentsTempDirFilePath = App.Current.TEMP_DIR_FILE_PATH + "/" + this.GetJarFileNameFromJarFilePath(
                    jarFilePath
                ) + "-jar-contents";

                if (Directory.Exists(jarContentsTempDirFilePath) == true)
                {
                    DirectoryInfo jarContentsTempDirInfo = new DirectoryInfo(
                        jarContentsTempDirFilePath
                    );

                    foreach (FileInfo fileInfo in jarContentsTempDirInfo.GetFiles())
                    {
                        fileInfo.Delete();
                    }

                    foreach (DirectoryInfo dirInfo in jarContentsTempDirInfo.GetDirectories())
                    {
                        dirInfo.Delete(true);
                    }
                }
                else
                {
                    Directory.CreateDirectory(jarContentsTempDirFilePath);
                }

                this.filePathsOfJarContentsTempDirsToClear.Add(
                    jarContentsTempDirFilePath
                );

                this.fastZip.ExtractZip(
                    jarFilePath,
                    jarContentsTempDirFilePath,
                    ""
                );
            }

            private void PerformCleanup()
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    App.Current.MainWindow.OpenFileLoadingPopupTextBlock.Text = MainWindow.LOADING_POPUP_CLEANUP_TEXT;
                });

                foreach (string jarContentsTempDirFilePath in this.filePathsOfJarContentsTempDirsToClear)
                {
                    new DirectoryInfo(jarContentsTempDirFilePath).Delete(true);
                }

                this.filePathsOfJarContentsTempDirsToClear.Clear();
            }

            private void End()
            {
                this.PerformCleanup();

                App.Current.Dispatcher.Invoke(() =>
                {
                    App.Current.MainWindow.OpenFileLoadingPopup.Visibility = Visibility.Collapsed;
                });
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                this.thread = Thread.CurrentThread;

                if (this.task == Task.OpenFile)
                {
                    foreach (string currentFilePath in this.filePaths)
                    {
                        string currentFileName = System.IO.Path.GetFileName(currentFilePath);

                        if (currentFileName.Contains(".class") == true)
                        {
                            this.OpenJavaClassFile(currentFilePath);
                        }
                        else if (currentFileName.Contains(".jar") == true)
                        {
                            this.OpenJarFile(currentFilePath);
                        }
                    }
                }

                this.End();
            }

            protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
            {

            }

            public OpenFileBackgroundWorker() : base()
            {
                this.WorkerSupportsCancellation = true;
            }

            public void RunWorkerAsync(Task task, string[] filePaths)
            {
                this.task = task;

                if (this.task == Task.OpenFile)
                {
                    this.filePaths = filePaths;
                }
                else if (this.task == Task.Cancel)
                {
                    this.filePathsOfJarContentsTempDirsToClear = new List<string>(
                        filePaths
                    );
                }

                base.RunWorkerAsync();
            }

            public new void CancelAsync()
            {
                base.CancelAsync();

                try
                {
                    this.thread.Abort();
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(e.ToString());
                }

                OpenFileBackgroundWorker openFileCancellationBackgroundWorker = new OpenFileBackgroundWorker();

                openFileCancellationBackgroundWorker.RunWorkerAsync(
                    Task.Cancel,
                    this.filePathsOfJarContentsTempDirsToClear.ToArray()
                );
            }

            public enum Task
            {
                OpenFile,
                Cancel
            }
        }
    }
}
