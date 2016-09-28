using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using JetBrains.Application.Settings;

namespace AgentSmith.Options
{
    public class WhitespaceListItem
    {
        public string Key { get; set; }
        public string Text { get; set; }
        public int Newlines { get; set; }
        public bool Indent { get; set; }
    }

    /// <summary>
    /// Interaction logic for ReflowAndRetagOptionsUI.xaml
    /// </summary>
    public partial class ReflowAndRetagOptionsUI : UserControl
    {

        private IContextBoundSettingsStore _settingsStore;
        public ReflowAndRetagOptionsUI(IContextBoundSettingsStore store)
        {
            _settingsStore = store;
            SetWhitespaceItems();
            InitializeComponent();

            
        }

        public ObservableCollection<WhitespaceListItem> WhitespaceListItems { get; set; }

        private void AddItem(string key, string text)
        {
            WhitespaceListItem item = new WhitespaceListItem()
            {
                Key = key,
                Text = text
            };
            item.Newlines = _settingsStore.GetIndexedValue<ReflowAndRetagSettings, string, int>(x => x.WhitespaceNewlineSettings,
                                                                                             item.Key + "OnNewLine");
            item.Indent = _settingsStore.GetIndexedValue<ReflowAndRetagSettings, string, bool>(x => x.WhitespaceIndentSettings,
                                                                                             item.Key + "Indent");
            WhitespaceListItems.Add(item);
        }

        private void SetWhitespaceItems()
        {
            WhitespaceListItems = new ObservableCollection<WhitespaceListItem>();
            AddItem("SummaryTag", "<summary>");
            AddItem("RemarksTag", "<remarks>");
            AddItem("ExampleTag", "<example>");
            AddItem("ReturnsTag", "<returns>");
            AddItem("ParaTag", "<para>");
            AddItem("ParamTag", "<param>");
            AddItem("TypeParamTag", "<typeparam>");
            AddItem("ListTag", "<list>");
            AddItem("ListHeaderTag", "<listheader>");
            AddItem("ItemTag", "<item>");
            AddItem("TermTag", "<term>");
            AddItem("DescriptionTag", "<description>");
            AddItem("ExceptionTag", "<exception>");
            AddItem("PermissionTag", "<permission>");
        }

        private void UpdateNewlineSetting(WhitespaceListItem item)
        {
            int value = _settingsStore.GetIndexedValue<ReflowAndRetagSettings, string, int>(x => x.WhitespaceNewlineSettings,
                                                                                             item.Key + "OnNewLine");
            if (item.Newlines != value)
                _settingsStore.SetIndexedValue<ReflowAndRetagSettings, string, int>(x => x.WhitespaceNewlineSettings,
                                                                                                 item.Key + "OnNewLine",
                                                                                                 item.Newlines);
        }

        private void UpdateIndentSetting(WhitespaceListItem item)
        {
            bool value = _settingsStore.GetIndexedValue<ReflowAndRetagSettings, string, bool>(x => x.WhitespaceIndentSettings,
                                                                                             item.Key + "Indent");
            if (item.Indent != value)
                _settingsStore.SetIndexedValue<ReflowAndRetagSettings, string, bool>(x => x.WhitespaceIndentSettings,
                                                                                                 item.Key + "Indent",
                                                                                                 item.Indent);
        }

        public void OnNewlinesChanged(object sender, SelectionChangedEventArgs args)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb == null) return;

            UpdateNewlineSetting((WhitespaceListItem) cmb.DataContext);

        }

        public void OnIndentChanged(object sender, RoutedEventArgs args)
        {
            CheckBox chk = sender as CheckBox;
            if (chk == null) return;

            UpdateIndentSetting((WhitespaceListItem)chk.DataContext);

        }
    }
}
