using System.Windows.Controls;

using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.Application;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;

namespace AgentSmith.Options {
	[OptionsPage(PID, "Resources", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class ResourceOptionsPage : AOptionsPage
	{

		public const string PID = "AgentSmithResourceId";

		private OptionsSettingsSmartContext _settings;

		private ResXOptionsUI _optionsUI;

		public ResourceOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID)
		{
			_settings = settingsSmartContext;
			_optionsUI = new ResXOptionsUI();
			this.Control = _optionsUI;

			settingsSmartContext.SetBinding<ResXSettings, string>(
				lifetime, x => x.DictionaryName, _optionsUI.txtDictionaryName, TextBox.TextProperty);
			settingsSmartContext.SetBinding<ResXSettings, string>(
				lifetime, x => x.WordsToIgnore, _optionsUI.txtWordsToIgnore, TextBox.TextProperty);

		}
	}
}