using System.ComponentModel;
using JetBrains.Annotations;
using JetBrains.Application.UI.Components;
using JetBrains.Application.UI.Options;
using Lifetime = JetBrains.Lifetimes.Lifetime;

namespace AgentSmith.Options {
	[OptionsPage(PID, "Reflow And Retag", typeof(OptionsThemedIcons.SamplePage), ParentId = XmlDocumentationOptionsPage.PID)]
	public class ReflowAndRetagOptionsPage : ReflowAndRetagOptionsUI, IOptionsPage { 


		public const string PID = "AgentSmithReflowAndRetagId";

		private OptionsSettingsSmartContext _settings;

		private ReflowAndRetagOptionsUI _optionsUI;

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

	}
}