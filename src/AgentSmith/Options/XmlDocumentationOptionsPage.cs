using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;

namespace AgentSmith.Options {
	[OptionsPage(PID, "Xml Documentation", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class XmlDocumentationOptionsPage : IOptionsPage {

		public const string PID = "AgentSmithXmlDocumentationId";

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