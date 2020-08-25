using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;

#if RESHARPER20172
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;
using JetBrains.Application.UI.UIAutomation;
#else
using JetBrains.UI.Options.OptionPages.ToolsPages;
#endif

namespace AgentSmith.Options {
	[OptionsPage(PID, "AgentSmith", typeof(OptionsThemedIcons.SamplePage), ParentId = ToolsPage.PID)]
#if RESHARPER20193
	public class AgentSmithOptionsPage : AEmptyOptionsPage {
#else
		public class AgentSmithOptionsPage : IOptionsPage {
#endif

		public const string PID = "AgentSmithId";

#if !RESHARPER20193
		#region Implementation of IOptionsPage

		public bool OnOk() {
			return true;
		}

		public bool ValidatePage() {
			return true;
		}

		public EitherControl Control { get { return null; } }

		public string Id { get { return PID; } }

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
#endif
	}
}