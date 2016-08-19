using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgentSmith.Options
{
    /// <summary>
    /// Interaction logic for EditCustomDictionaryDialog.xaml
    /// </summary>
    public partial class EditCustomDictionaryDialog : Window
    {
        public EditCustomDictionaryDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
