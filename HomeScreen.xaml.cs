using ICSharpCode.SharpZipLib.Zip;
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
        public Dictionary<JavaFileTreeViewItem, JavaClassFile> javaClassFileTreeViewItemToJavaClassFileMap = new Dictionary<JavaFileTreeViewItem, JavaClassFile>();

        private void OnMainTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.MainTreeView.SelectedItem == null)
            {
                this.SelectedFileTabControl.Visibility = Visibility.Collapsed;

                return;
            }

            JavaClassFile selectedJavaClassFile = this.javaClassFileTreeViewItemToJavaClassFileMap[this.MainTreeView.SelectedItem as JavaFileTreeViewItem];

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

            this.OnMainTreeViewSelectedItemChanged(null, null);

            this.MainTreeView.SelectedItemChanged += this.OnMainTreeViewSelectedItemChanged;
        }
    }
}
