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
using System.Xml.Serialization;

namespace Java_Bytecode_Toolkit
{
    public class JavaClassFileTreeViewItem : TreeViewItem
    {
        private MenuItem closeContextMenuItem = null;

        private MenuItem exportAsXMLFileContextMenuItem = null;

        private SaveFileDialog exportAsXMLFileSaveFileDialog = new SaveFileDialog();

        static JavaClassFileTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(JavaClassFileTreeViewItem), new FrameworkPropertyMetadata(typeof(JavaClassFileTreeViewItem)));
        }

        private void OnExportAsXMLFileSaveFileDialogFileOk(object sender, CancelEventArgs e)
        {
            App.Current.MainWindow.homeScreen.javaClassFileTreeViewItemToJavaClassFileMap[this].ExportAsXMLFile(
                this.exportAsXMLFileSaveFileDialog.FileName
            );
        }

        private void OnExportAsXMLFileContextMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.exportAsXMLFileSaveFileDialog.ShowDialog();
        }

        private void OnCloseContextMenuItemClick(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.homeScreen.MainTreeView.Items.Remove(this);
        }

        public JavaClassFileTreeViewItem()
        {

        }

        public override void OnApplyTemplate()
        {
            this.closeContextMenuItem = (MenuItem)this.Template.FindName(
                "CloseContextMenuItem",
                this
            );

            this.exportAsXMLFileContextMenuItem = (MenuItem)this.Template.FindName(
                "ExportAsXMLFileContextMenuItem",
                this
            );

            this.exportAsXMLFileSaveFileDialog.SetFilter(
                FileDialogFilter.XML_FILE_FILTER
            );

            this.closeContextMenuItem.Click += this.OnCloseContextMenuItemClick;

            this.exportAsXMLFileContextMenuItem.Click += this.OnExportAsXMLFileContextMenuItemClick;

            this.exportAsXMLFileSaveFileDialog.FileOk += this.OnExportAsXMLFileSaveFileDialogFileOk;
        }
    }
}
