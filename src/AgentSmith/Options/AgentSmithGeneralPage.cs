using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using JetBrains.Application.UI.Options;
using Lifetime = JetBrains.Lifetimes.Lifetime;


namespace AgentSmith.Options {
	[OptionsPage(PID, "General", typeof(OptionsThemedIcons.SamplePage), ParentId = AgentSmithOptionsPage.PID)]

	public class AgentSmithGeneralPage : AgentSmithOptionsUI, IOptionsPage {

		public const string PID = "AgentSmithGeneralId";

		private OptionsSettingsSmartContext _settings;

		private AgentSmithOptionsUI _optionsUI;

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


		private void InitializeOptionsUI() {
			AssemblyName assemblyName = typeof(AgentSmithOptionsPage).Assembly.GetName();
			_optionsUI.txtTitle.Text += assemblyName.Name + " V" + assemblyName.Version;
		}
	}
}