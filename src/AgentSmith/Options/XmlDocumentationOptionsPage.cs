using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Application.UI.Options;


namespace AgentSmith.Options {
	[OptionsPage(PID, "Xml Documentation", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class XmlDocumentationOptionsPage : AEmptyOptionsPage {

		public const string PID = "AgentSmithXmlDocumentationId";

	}
}