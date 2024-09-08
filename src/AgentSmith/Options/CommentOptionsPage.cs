using System.ComponentModel;
using System.Windows.Controls;

using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.DataFlow;
using JetBrains.UI.DataFlow;
using Lifetime = JetBrains.Lifetimes.Lifetime;


namespace AgentSmith.Options {
	[OptionsPage(PID, "Inline Comments", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class CommentOptionsPage : CommentOptionsUI, IOptionsPage {

		public const string PID = "AgentSmithInlineCommentId";

		private OptionsSettingsSmartContext _settings;

		private CommentOptionsUI _optionsUI;

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

        private void InitializeOptionsUI(Lifetime lifetime)
        {
            _settings.SetBinding<CommentSettings, string>(lifetime, x => x.DictionaryName,
                DependencyPropertyWrapper.Create<string>(lifetime, _optionsUI.txtDictionaryName, TextBox.TextProperty,
                    true));
            _settings.SetBinding<CommentSettings, string>(lifetime, x => x.WordsToIgnore,
                DependencyPropertyWrapper.Create<string>(lifetime, _optionsUI.txtWordsToIgnore, TextBox.TextProperty,
                    true));
        }

    }
}