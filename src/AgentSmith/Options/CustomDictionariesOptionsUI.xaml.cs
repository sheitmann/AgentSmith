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

#if !RESHARPER20172
using JetBrains.Application.Settings.Store.Implementation;
#endif

using JetBrains.UI.Controls;
using JetBrains.UI.CrossFramework;

namespace AgentSmith.Options
{
    /// <summary>
    /// Interaction logic for CustomDictionariesOptionsUI.xaml
    /// </summary>
    public partial class CustomDictionariesOptionsUI : UserControl 
    {
        public CustomDictionariesOptionsUI()
        {
            InitializeComponent();
        }


    }
}
