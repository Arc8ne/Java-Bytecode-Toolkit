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

namespace Java_Bytecode_Toolkit
{
    /// <summary>
    /// Interaction logic for SettingsScreen.xaml
    /// </summary>
    public partial class SettingsScreen : UserControl
    {
        private void OnEnableLoggingCheckBoxIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            App.Current.configuration.EnableLogging = this.EnableLoggingCheckBox.IsEnabled;
        }

        private void OnDeleteAllLogsButtonClick(object sender, RoutedEventArgs e)
        {
            App.Current.logger.DeleteAllLogs();
        }

        public SettingsScreen()
        {
            InitializeComponent();

            this.DataContext = App.Current.configuration;

            this.EnableLoggingCheckBox.IsEnabledChanged += this.OnEnableLoggingCheckBoxIsEnabledChanged;

            this.DeleteAllLogsButton.Click += this.OnDeleteAllLogsButtonClick;
        }
    }
}
