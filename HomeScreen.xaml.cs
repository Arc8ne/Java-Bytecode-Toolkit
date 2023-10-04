using Java_Bytecode_Toolkit.ExtensionsNS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        private OpenFileDialog openFileDialog = new OpenFileDialog();

        public Dictionary<JavaClassFileTreeViewItem, JavaClassFile> javaClassFileTreeViewItemToJavaClassFileMap = new Dictionary<JavaClassFileTreeViewItem, JavaClassFile>();

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

        private void OnOpenFileDialogFileOk(object sender, CancelEventArgs e)
        {
            this.OpenFile(this.openFileDialog.FileNames);
        }

        private void OpenJavaClassFile(string javaClassFilePath)
        {
            JavaClassFile javaClassFile = new JavaClassFile(javaClassFilePath);

            JavaClassFileTreeViewItem javaClassFileTreeViewItem = new JavaClassFileTreeViewItem()
            {
                DataContext = javaClassFile
            };

            this.MainTreeView.Items.Add(
                javaClassFileTreeViewItem
            );

            this.javaClassFileTreeViewItemToJavaClassFileMap[javaClassFileTreeViewItem] = javaClassFile;

            javaClassFileTreeViewItem.IsSelected = true;
        }

        private void OpenJarFile(string jarFilePath)
        {

        }

        private void OpenFile(params string[] filePaths)
        {
            foreach (string currentFilePath in filePaths)
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

        private void OnMainTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.MainTreeView.SelectedItem == null)
            {
                this.SelectedFileTabControl.Visibility = Visibility.Collapsed;

                return;
            }

            JavaClassFile selectedJavaClassFile = this.javaClassFileTreeViewItemToJavaClassFileMap[this.MainTreeView.SelectedItem as JavaClassFileTreeViewItem];

            this.ClassNameTextBlock.Text = selectedJavaClassFile.Name.Split('.')[0];

            this.FilePathTextBlock.Text = selectedJavaClassFile.filePath;

            this.FileMagicNumberInHexTextBlock.Text = "0x" + selectedJavaClassFile.magic.ToString("X");

            this.MajorVersionTextBlock.Text = selectedJavaClassFile.majorVersion.ToString();

            this.MinorVersionTextBlock.Text = selectedJavaClassFile.minorVersion.ToString();

            this.ConstantPoolSizeTextBlock.Text = selectedJavaClassFile.constantPoolCount.ToString();

            this.AccessFlagsTextBlock.Text = selectedJavaClassFile.accessFlags.ToString();

            this.NumInterfacesTextBlock.Text = selectedJavaClassFile.interfacesCount.ToString();

            this.NumFieldsTextBlock.Text = selectedJavaClassFile.fieldsCount.ToString();

            this.NumMethodsTextBlock.Text = selectedJavaClassFile.methodsCount.ToString();

            this.NumAttribsTextBlock.Text = selectedJavaClassFile.attributesCount.ToString();

            this.SelectedFileTabControl.Visibility = Visibility.Visible;
        }

        public HomeScreen()
        {
            InitializeComponent();

            this.openFileDialog.SetFilter(
                FileDialogFilter.JAVA_CLASS_FILE_FILTER,
                FileDialogFilter.JAR_FILE_FILTER
            );

            this.OnMainTreeViewSelectedItemChanged(null, null);

            this.OpenFileMenuItem.Click += this.OnOpenFileMenuItemClick;

            this.openFileDialog.FileOk += this.OnOpenFileDialogFileOk;

            this.LightThemeMenuItem.Click += this.OnLightThemeMenuItemClick;

            this.DarkThemeMenuItem.Click += this.OnDarkThemeMenuItemClick;

            this.MainTreeView.SelectedItemChanged += this.OnMainTreeViewSelectedItemChanged;
        }
    }
}
