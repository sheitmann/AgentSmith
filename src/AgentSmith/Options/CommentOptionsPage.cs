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

namespace AgentSmith.Options {
	[OptionsPage(PID, "Inline Comments", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class CommentOptionsPage : AOptionsPage
	{

		public const string PID = "AgentSmithInlineCommentId";

		private OptionsSettingsSmartContext _settings;

		private CommentOptionsUI _optionsUI;

		public CommentOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID)
		{
			_settings = settingsSmartContext;
			_optionsUI = new CommentOptionsUI();
			Control = _optionsUI;

			settingsSmartContext.SetBinding<CommentSettings, string>(
				lifetime, x => x.DictionaryName, _optionsUI.txtDictionaryName, TextBox.TextProperty);
			settingsSmartContext.SetBinding<CommentSettings, string>(
				lifetime, x => x.WordsToIgnore, _optionsUI.txtWordsToIgnore, TextBox.TextProperty);

		}
	}
}