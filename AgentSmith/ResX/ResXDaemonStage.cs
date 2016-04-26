using System;
using System.Collections.Generic;

using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace AgentSmith.ResX
{
    [DaemonStage(StagesBefore=new Type[] { typeof(UnsafeContextCheckingStage) })]
    public class ResXDaemonStage : IDaemonStage
    {
        #region IDaemonStage Members

	    public IEnumerable<IDaemonStageProcess> CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind) {
			if (process.SourceFile.Name.ToLower().EndsWith(".resx")) {
				yield return new ResXProcess(process, settings, process.SourceFile);
			}
	    }

	    public ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        {
            return ErrorStripeRequest.STRIPE_AND_ERRORS;
        }

        #endregion
    }
}