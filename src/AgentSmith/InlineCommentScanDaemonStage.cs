using System.Collections.Generic;

using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith
{
    /// <summary>
    /// A stage (process stage factory) which creates process stages for scanning inline (code) comments
    /// </summary>
    [DaemonStage(StagesBefore = new[] { typeof(LanguageSpecificDaemonStage) })]
    internal class InlineCommentScanDaemonStage : IDaemonStage
    {
	    public IEnumerable<IDaemonStageProcess> CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind) {
			IFile psiFile = process.SourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance);
		    if (psiFile != null) {
			    yield return new InlineCommentScanDaemonStageProcess(process, settings);
		    }
	    }

	    /// <summary>
        /// Check the error stripe indicator necessity for this stage after processing given <paramref name="sourceFile"/>
        /// </summary>
        public ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        {
            return ErrorStripeRequest.STRIPE_AND_ERRORS;
        }
    }
}