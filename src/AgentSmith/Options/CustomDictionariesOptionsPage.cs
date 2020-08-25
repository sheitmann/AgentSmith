using System.ComponentModel;
using System.Windows;

using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.Application;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;

#if RESHARPER20172
using JetBrains.Application.UI.Components;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;
using JetBrains.Application.UI.UIAutomation;
#else
using JetBrains.UI.Options.OptionPages.ToolsPages;
#endif

#if RESHARPER20191
using Lifetime = JetBrains.Lifetimes.Lifetime;
#else
    using Lifetime = JetBrains.DataFlow.Lifetime;
#endif

namespace AgentSmith.Options {
    [OptionsPage(PID, "User Dictionaries", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
#if RESHARPER20193
    public class CustomDictionariesOptionsPage : CustomDictionariesOptionsUI, IOptionsPage {
#else
        public class CustomDictionariesOptionsPage : AOptionsPage {
#endif

        private const string PID = "AgentSmithUserDictionariesId";

        private OptionsSettingsSmartContext _settings;

        private CustomDictionariesOptionsUI _optionsUI;

#if RESHARPER20193

        public CustomDictionariesOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext) {
		    _settings = settingsSmartContext;
		    _optionsUI = this;

		    InitializeOptionsUI();
        }

        #region Implementation of INotifyPropertyChanged

	    public event PropertyChangedEventHandler PropertyChanged;

	    #endregion

	    #region Implementation of IOptionsPage

	    public bool OnOk() => true;

	    public string Id => PID;

        #endregion
#else

        public CustomDictionariesOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
            : base(lifetime, environment, PID) {
            _settings = settingsSmartContext;
            _optionsUI = new CustomDictionariesOptionsUI();

            this.Control = _optionsUI;

            InitializeOptionsUI();
        }
#endif

        private void InitializeOptionsUI() {
            RefreshCustomDictionaryList();

            _optionsUI.btnAdd.Click += BtnAddOnClick;
            _optionsUI.btnEdit.Click += BtnEditOnClick;
            _optionsUI.btnDelete.Click += BtnDeleteOnClick;
            /*
		    settingsSmartContext.SetBinding<SpellCheckSettings, IEnumerable>(
		        lifetime, x => (IEnumerable)x.CustomDictionaries, optionsUI.lstCustomDictionaries, ListView.ItemsSourceProperty);
		     */
        }

        private void RefreshCustomDictionaryList() {
            _optionsUI.lstCustomDictionaries.Items.Clear();
            foreach (string item in _settings.EnumEntryIndices<CustomDictionarySettings, string, CustomDictionary>(
                x => x.CustomDictionaries)) {
                _optionsUI.lstCustomDictionaries.Items.Add(item);
            }
        }

        private string GetSelectedDictionaryName() {
            return (string)_optionsUI.lstCustomDictionaries.SelectedItem;
        }

        private CustomDictionary GetDictionary(string name) {
            return _settings.GetIndexedValue<CustomDictionarySettings, string, CustomDictionary>(
                x => x.CustomDictionaries, name);
        }
        private void SetDictionary(string name, CustomDictionary dictionary) {
            _settings.SetIndexedValue<CustomDictionarySettings, string, CustomDictionary>(
                x => x.CustomDictionaries, name, dictionary);
        }
        private void RemoveDictionary(string name) {
            _settings.RemoveIndexedValue<CustomDictionarySettings, string, CustomDictionary>(settings => settings.CustomDictionaries, name);
        }

        private void BtnDeleteOnClick(object sender, RoutedEventArgs routedEventArgs) {
            string dictName = GetSelectedDictionaryName();

            RemoveDictionary(dictName);

            RefreshCustomDictionaryList();
            SpellCheckManager.Reset(); // Clear the cache.
        }

        private void BtnEditOnClick(object sender, RoutedEventArgs routedEventArgs) {
            string dictName = GetSelectedDictionaryName();

            CustomDictionary dict = GetDictionary(dictName);

            EditCustomDictionaryDialog dlg = new EditCustomDictionaryDialog();

            dlg.txtName.Text = dict.Name;
            dlg.txtUserWords.Text = dict.DecodedUserWords;
            dlg.chkCaseSensitive.IsChecked = dict.CaseSensitive;

            if (dlg.ShowDialog() == true) {
                bool changes = false;
                if (dlg.txtName.Text != dict.Name) {
                    RemoveDictionary(dict.Name);
                    dict.Name = dlg.txtName.Text;
                    changes = true;
                }
                if (dict.DecodedUserWords != dlg.txtUserWords.Text) {
                    dict.DecodedUserWords = dlg.txtUserWords.Text;
                    changes = true;
                }

                if (dict.CaseSensitive != dlg.chkCaseSensitive.IsChecked) {
                    dict.CaseSensitive = (bool)dlg.chkCaseSensitive.IsChecked;
                    changes = true;
                }

                if (changes) {
                    SetDictionary(dict.Name, dict);
                }
                RefreshCustomDictionaryList();
                SpellCheckManager.Reset(); // Clear the cache.
            }
        }

        void BtnAddOnClick(object sender, System.Windows.RoutedEventArgs e) {
            EditCustomDictionaryDialog dlg = new EditCustomDictionaryDialog();

            dlg.Title = "Add Custom Dictionary";

            if (dlg.ShowDialog() == true) {
                CustomDictionary dict = new CustomDictionary();
                dict.Name = dlg.txtName.Text;
                dict.DecodedUserWords = dlg.txtUserWords.Text;
                dict.CaseSensitive = (bool)dlg.chkCaseSensitive.IsChecked;

                SetDictionary(dict.Name, dict);
                RefreshCustomDictionaryList();
                SpellCheckManager.Reset(); // Clear the cache.
            }
        }
    }
}