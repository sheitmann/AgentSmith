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
	[OptionsPage(PID, "Inline Comments", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
#if RESHARPER20193
	public class CommentOptionsPage : CommentOptionsUI, IOptionsPage {
#else
	public class CommentOptionsPage : AOptionsPage {
#endif
		public const string PID = "AgentSmithInlineCommentId";

		private OptionsSettingsSmartContext _settings;

		private CommentOptionsUI _optionsUI;

#if RESHARPER20193
		public CommentOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext) {
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

		public CommentOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID) {
			_settings = settingsSmartContext;
			_optionsUI = new CommentOptionsUI();
			Control = _optionsUI;

			InitializeOptionsUI(lifetime);
		}
#endif
		private void InitializeOptionsUI(Lifetime lifetime) {
			_settings.SetBinding<CommentSettings, string>(
				lifetime, x => x.DictionaryName, _optionsUI.txtDictionaryName, TextBox.TextProperty);
			_settings.SetBinding<CommentSettings, string>(
				lifetime, x => x.WordsToIgnore, _optionsUI.txtWordsToIgnore, TextBox.TextProperty);
		}

	}
}