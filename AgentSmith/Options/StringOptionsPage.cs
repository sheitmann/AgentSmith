using System.Windows.Controls;

using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.Application;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;

namespace AgentSmith.Options {
	[OptionsPage(PID, "Strings", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class StringOptionsPage : AOptionsPage
	{

		public const string PID = "AgentSmithStringId";

		private OptionsSettingsSmartContext _settings;

		private StringOptionsUI _optionsUI;

		public StringOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID)
		{
			_settings = settingsSmartContext;
			_optionsUI = new StringOptionsUI();
			this.Control = _optionsUI;

			settingsSmartContext.SetBinding<StringSettings, string>(
				lifetime, x => x.DictionaryName, _optionsUI.txtDictionaryName, TextBox.TextProperty);
			settingsSmartContext.SetBinding<StringSettings, bool?>(
				lifetime, x => x.IgnoreVerbatimStrings, _optionsUI.chkIgnoreVerbatimStrings, CheckBox.IsCheckedProperty);
			settingsSmartContext.SetBinding<StringSettings, string>(
				lifetime, x => x.WordsToIgnore, _optionsUI.txtWordsToIgnore, TextBox.TextProperty);

		}
	}
}