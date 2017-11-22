using System.Reflection;

using JetBrains.Annotations;
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
	[OptionsPage(PID, "General", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]
	public class AgentSmithGeneralPage : AOptionsPage {

		public const string PID = "AgentSmithGeneralId";

		private OptionsSettingsSmartContext _settings;

		private AgentSmithOptionsUI _optionsUI;

		public AgentSmithGeneralPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID) {
			_settings = settingsSmartContext;
			_optionsUI = new AgentSmithOptionsUI();

			AssemblyName assemblyName = typeof(AgentSmithOptionsPage).Assembly.GetName();

			_optionsUI.txtTitle.Text += assemblyName.Name + " V" + assemblyName.Version;
			Control = _optionsUI;
		}
	}
}