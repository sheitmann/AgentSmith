using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionPages.ToolsPages;

namespace AgentSmith.Options {
	[OptionsPage(PID, "AgentSmith", typeof(OptionsThemedIcons.SamplePage), ParentId = ToolsPage.PID)]
	public class AgentSmithOptionsPage : IOptionsPage
	{

		public const string PID = "AgentSmithId";

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
	}
}