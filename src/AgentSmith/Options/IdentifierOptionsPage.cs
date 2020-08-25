using System.ComponentModel;
using System.Windows.Controls;

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
	[OptionsPage(PID, "Identifiers", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]

#if RESHARPER20193
	public class IdentifierOptionsPage : IdentifierOptionsUI, IOptionsPage {
#else
	public class IdentifierOptionsPage : AOptionsPage {
#endif

		public const string PID = "AgentSmithIdentifierId";

		private OptionsSettingsSmartContext _settings;

		private IdentifierOptionsUI _optionsUI;
#if RESHARPER20193
		public IdentifierOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment) {
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
#else
		public IdentifierOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID) {
			_settings = settingsSmartContext;
			_optionsUI = new IdentifierOptionsUI();
			this.Control = _optionsUI;

			InitializeOptionsUI(lifetime);
		}
#endif

		private void InitializeOptionsUI(Lifetime lifetime) {
			_settings.SetBinding<IdentifierSettings, string>(
				lifetime, x => x.DictionaryName, _optionsUI.txtDictionaryName, TextBox.TextProperty);
			_settings.SetBinding<IdentifierSettings, string>(
				lifetime, x => x.WordsToIgnore, _optionsUI.txtWordsToIgnore, TextBox.TextProperty);
			_settings.SetBinding<IdentifierSettings, int>(
				lifetime, x => x.LookupScope, _optionsUI.cmbLookupScope, ComboBox.SelectedIndexProperty);
		}
	}
}