using System.ComponentModel;
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
	[OptionsPage(PID, "Reflow And Retag", typeof(OptionsThemedIcons.SamplePage), ParentId = XmlDocumentationOptionsPage.PID)]
#if RESHARPER20193
	public class ReflowAndRetagOptionsPage : ReflowAndRetagOptionsUI, IOptionsPage { 
#else
		public class ReflowAndRetagOptionsPage : AOptionsPage {
#endif

		public const string PID = "AgentSmithReflowAndRetagId";

		private OptionsSettingsSmartContext _settings;

		private ReflowAndRetagOptionsUI _optionsUI;
#if RESHARPER20193
		public ReflowAndRetagOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment) : base(settingsSmartContext) {
			_settings = settingsSmartContext;
			_optionsUI =this;
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Implementation of IOptionsPage

		public bool OnOk() => true;

		public string Id => PID;

		#endregion
#else
		public ReflowAndRetagOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settingsSmartContext, IUIApplication environment)
			: base(lifetime, environment, PID) {
			_settings = settingsSmartContext;
			_optionsUI = new ReflowAndRetagOptionsUI(_settings);
			this.Control = _optionsUI;
		}
#endif
	}
}