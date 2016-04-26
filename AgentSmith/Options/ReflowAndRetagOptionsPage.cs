using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.UI.Application;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;

namespace AgentSmith.Options {
	[OptionsPage(PID, "Reflow And Retag", typeof(OptionsThemedIcons.SamplePage), ParentId = XmlDocumentationOptionsPage.PID)]
	public class ReflowAndRetagOptionsPage : AOptionsPage
	{

		public const string PID = "AgentSmithReflowAndRetagId";

		private OptionsSettingsSmartContext _settings;

		private ReflowAndRetagOptionsUI _optionsUI;

		public ReflowAndRetagOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID)
		{
			_settings = settingsSmartContext;
			_optionsUI = new ReflowAndRetagOptionsUI(_settings);
			this.Control = _optionsUI;

		}


	}
}