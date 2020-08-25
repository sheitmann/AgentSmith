using System.ComponentModel;
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

#if RESHARPER20191
    using Lifetime = JetBrains.Lifetimes.Lifetime;
#else
    using Lifetime = JetBrains.DataFlow.Lifetime;
#endif

namespace AgentSmith.Options {
	[OptionsPage(PID, "General", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]

#if RESHARPER20193
	public class AgentSmithGeneralPage : AgentSmithOptionsUI, IOptionsPage {
#else
	public class AgentSmithGeneralPage : AOptionsPage {
#endif
		public const string PID = "AgentSmithGeneralId";

		private OptionsSettingsSmartContext _settings;

		private AgentSmithOptionsUI _optionsUI;

#if RESHARPER20193

		public AgentSmithGeneralPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext) {
			_settings = settingsSmartContext;
			_optionsUI = new AgentSmithOptionsUI();

			InitializeOptionsUI();
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Implementation of IOptionsPage

		public bool OnOk() => true;

		public string Id => PID;

		#endregion
#else
		public AgentSmithGeneralPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID) {
			_settings = settingsSmartContext;
			_optionsUI = new AgentSmithOptionsUI();

			Control = _optionsUI;

			InitializeOptionsUI();
		}
#endif

		private void InitializeOptionsUI() {
			AssemblyName assemblyName = typeof(AgentSmithOptionsPage).Assembly.GetName();
			_optionsUI.txtTitle.Text += assemblyName.Name + " V" + assemblyName.Version;
		}
	}
}