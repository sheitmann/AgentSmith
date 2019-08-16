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
	[OptionsPage(PID, "General", typeof(OptionsThemedIcons.SamplePage), ParentId = XmlDocumentationOptionsPage.PID)]
	public class XmlDocumentationGeneralOptionsPage : AOptionsPage {

		public const string PID = "AgentSmithXmlDocumentationGEneralId";

		private OptionsSettingsSmartContext _settings;

		private XmlDocumentationOptionsUI _optionsUI;

		public XmlDocumentationGeneralOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID) {
			_settings = settingsSmartContext;
			_optionsUI = new XmlDocumentationOptionsUI();
			this.Control = _optionsUI;

			settingsSmartContext.SetBinding<XmlDocumentationSettings, string>(
				lifetime, x => x.DictionaryName, _optionsUI.txtDictionaryName, TextBox.TextProperty);
			settingsSmartContext.SetBinding<XmlDocumentationSettings, bool?>(
				lifetime, x => x.SuppressIfBaseHasComment, _optionsUI.chkSuppressIfBaseHasComment, CheckBox.IsCheckedProperty);
			settingsSmartContext.SetBinding<XmlDocumentationSettings, int>(
				lifetime, x => x.MaxCharactersPerLine, _optionsUI.txtMaxCharsPerLine, IntegerTextBox.ValueProperty);
			settingsSmartContext.SetBinding<XmlDocumentationSettings, string>(
				lifetime, x => x.WordsToIgnore, _optionsUI.txtWordsToIgnore, TextBox.TextProperty);
			settingsSmartContext.SetBinding<XmlDocumentationSettings, string>(
				lifetime, x => x.WordsToIgnoreForMetatagging, _optionsUI.txtWordsToIgnoreForMetatagging, TextBox.TextProperty);
			settingsSmartContext.SetBinding<XmlDocumentationSettings, string>(
				lifetime, x => x.ProjectNamesToIgnore, _optionsUI.txtProjectNamesToIgnore, TextBox.TextProperty);

		}
	}
}