using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;


namespace AgentSmith.Options {
	[OptionsPage(PID, "AgentSmith", typeof(OptionsThemedIcons.SamplePage), ParentId = ToolsPage.PID)]

	public class AgentSmithOptionsPage : AEmptyOptionsPage {


		public const string PID = "AgentSmithId";

	}
}