using System.ComponentModel;
using System.Windows.Controls;

using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.Application;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;

using JetBrains.Application.UI.Components;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;
using JetBrains.Application.UI.UIAutomation;



using Lifetime = JetBrains.Lifetimes.Lifetime;
using JetBrains.UI.DataFlow;
using static JetBrains.UI.Options.Helpers.Controls;
using CheckBox = System.Windows.Controls.CheckBox;


namespace AgentSmith.Options {
	[OptionsPage(PID, "Strings", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class StringOptionsPage : StringOptionsUI, IOptionsPage {

		public const string PID = "AgentSmithStringId";

		private OptionsSettingsSmartContext _settings;

		private StringOptionsUI _optionsUI;

		public StringOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment) {
			_settings = settingsSmartContext;
			_optionsUI = this;

			InitializeOptionsUI(lifetime);
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Implementation of IOptionsPage

		public bool OnOk() => true;

		public string Id => PID;

		#endregion

        private void InitializeOptionsUI(Lifetime lifetime)
        {
            _settings.SetBinding<StringSettings, string>(lifetime, x => x.DictionaryName,
                DependencyPropertyWrapper.Create<string>(lifetime, _optionsUI.txtDictionaryName, TextBox.TextProperty,
                    true));
            _settings.SetBinding<StringSettings, bool?>(lifetime, x => x.IgnoreVerbatimStrings,
                DependencyPropertyWrapper.Create<bool?>(lifetime, _optionsUI.chkIgnoreVerbatimStrings,
                    CheckBox.IsCheckedProperty, true));
            _settings.SetBinding<StringSettings, string>(lifetime, x => x.WordsToIgnore,
                DependencyPropertyWrapper.Create<string>(lifetime, _optionsUI.txtWordsToIgnore, TextBox.TextProperty,
                    true));
        }

    }
}