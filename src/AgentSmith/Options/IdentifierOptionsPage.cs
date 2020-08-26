using System.ComponentModel;
using System.Windows.Controls;

using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Components;
using JetBrains.Application.UI.Options;
using Lifetime = JetBrains.Lifetimes.Lifetime;


namespace AgentSmith.Options {
	[OptionsPage(PID, "Identifiers", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]

	public class IdentifierOptionsPage : IdentifierOptionsUI, IOptionsPage {


		public const string PID = "AgentSmithIdentifierId";

		private OptionsSettingsSmartContext _settings;

		private IdentifierOptionsUI _optionsUI;

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