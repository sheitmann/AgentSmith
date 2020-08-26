using System.ComponentModel;
using System.Windows.Controls;

using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Components;
using JetBrains.Application.UI.Options;
using Lifetime = JetBrains.Lifetimes.Lifetime;


namespace AgentSmith.Options {
	[OptionsPage(PID, "Resources", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class ResourceOptionsPage : ResXOptionsUI, IOptionsPage {


		public const string PID = "AgentSmithResourceId";

		private OptionsSettingsSmartContext _settings;

		private ResXOptionsUI _optionsUI;

		public ResourceOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment) {
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
			_settings.SetBinding<ResXSettings, string>(
				lifetime, x => x.DictionaryName, _optionsUI.txtDictionaryName, TextBox.TextProperty);
			_settings.SetBinding<ResXSettings, string>(
				lifetime, x => x.WordsToIgnore, _optionsUI.txtWordsToIgnore, TextBox.TextProperty);
		}
	}
}