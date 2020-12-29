using System.Collections.Generic;

using AgentSmith.Options;

using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
//using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith.Analyzers {

	//[ElementProblemAnalyzer(typeof(ICSharpLiteralExpression))]
	//public class StringLiteralTypoAnalyzer : ElementProblemAnalyzer<ICSharpLiteralExpression> {
	//	private readonly ISolution m_solution;

	//	private readonly ISettingsStore m_settingsStore;

	//	private IContextBoundSettingsStore m_contextBoundSettingsStore;

	//	public StringLiteralTypoAnalyzer(ISolution solution, ISettingsStore settingsStore) {
	//		m_solution = solution;
	//		m_settingsStore = settingsStore;
	//		m_contextBoundSettingsStore  = m_settingsStore.BindToContextTransient(ContextRange.ApplicationWide);
	//	}

	//	#region Overrides of ElementProblemAnalyzer<ICSharpLiteralExpression>

	//	protected override void Run(ICSharpLiteralExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer) {
	//		var highlightings = new List<HighlightingInfo>();
	//		StringSettings stringSettings = m_contextBoundSettingsStore.GetKey<StringSettings>(SettingsOptimization.OptimizeDefault); 
	//		StringLiteralScanDaemonStageProcess.CheckString(element, highlightings, stringSettings, m_solution, m_contextBoundSettingsStore);
	//		consumer.AddHighlightings(element.GetContainingFile(), highlightings);
	//	}

	//	#endregion
	//}
}