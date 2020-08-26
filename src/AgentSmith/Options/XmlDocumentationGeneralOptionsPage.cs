using System.ComponentModel;
using System.Windows.Controls;

using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Components;
using JetBrains.Application.UI.Options;
using Lifetime = JetBrains.Lifetimes.Lifetime;


namespace AgentSmith.Options {
	[OptionsPage(PID, "General", typeof(OptionsThemedIcons.SamplePage), ParentId = XmlDocumentationOptionsPage.PID)]
	public class XmlDocumentationGeneralOptionsPage : XmlDocumentationOptionsUI, IOptionsPage {


		public const string PID = "AgentSmithXmlDocumentationGEneralId";

		private OptionsSettingsSmartContext _settings;

		private XmlDocumentationOptionsUI _optionsUI;

		public XmlDocumentationGeneralOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment) {
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
			_settings.SetBinding<XmlDocumentationSettings, string>(
				lifetime, x => x.DictionaryName, _optionsUI.txtDictionaryName, TextBox.TextProperty);
			_settings.SetBinding<XmlDocumentationSettings, bool?>(
				lifetime, x => x.SuppressIfBaseHasComment, _optionsUI.chkSuppressIfBaseHasComment, CheckBox.IsCheckedProperty);
			_settings.SetBinding<XmlDocumentationSettings, int>(
				lifetime, x => x.MaxCharactersPerLine, _optionsUI.txtMaxCharsPerLine, IntegerTextBox.ValueProperty);
			_settings.SetBinding<XmlDocumentationSettings, string>(
				lifetime, x => x.WordsToIgnore, _optionsUI.txtWordsToIgnore, TextBox.TextProperty);
			_settings.SetBinding<XmlDocumentationSettings, string>(
				lifetime, x => x.WordsToIgnoreForMetatagging, _optionsUI.txtWordsToIgnoreForMetatagging, TextBox.TextProperty);
			_settings.SetBinding<XmlDocumentationSettings, string>(
				lifetime, x => x.ProjectNamesToIgnore, _optionsUI.txtProjectNamesToIgnore, TextBox.TextProperty);
		}

	}
}