using System;
using System.Collections.Generic;

using AgentSmith.Options;
using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;
using AgentSmith.Strings;

using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Services;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentSmith
{
    public class StringLiteralScanDaemonStageProcess : IDaemonStageProcess
    {
        /// <summary>
        /// Internal storage for the process that this stage is a part of
        /// </summary>
        private readonly IDaemonProcess _daemonProcess;
        private readonly ISolution _solution;

        private readonly IContextBoundSettingsStore _settingsStore;

        /// <summary>
        /// Create a new process within a stage that processes comments in the current file.
        /// </summary>
        /// <param name="daemonProcess">The current instance process that this stage will be a part of</param>
        /// <param name="settingsStore"> </param>
        public StringLiteralScanDaemonStageProcess(IDaemonProcess daemonProcess, IContextBoundSettingsStore settingsStore)
        {
            this._daemonProcess = daemonProcess;
            this._solution = daemonProcess.Solution;

            this._settingsStore = settingsStore;


        }

        #region IDaemonStageProcess Members

        /// <summary>
        /// The current instance process that this stage is a part of.
        /// </summary>
        public IDaemonProcess DaemonProcess { get { return this._daemonProcess; } }

        /// <summary>
        /// Execute this stage of the process.
        /// </summary>
        /// <param name="commiter">The function to call when we've finished the stage to report the results.</param>
        public void Execute(Action<DaemonStageResult> commiter)
        {

			var consumer = new DefaultHighlightingConsumer(this, _settingsStore);  

            IFile file = this._daemonProcess.SourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance);
            if (file == null)
            {
                return;
            }

            StringSettings stringSettings = this._settingsStore.GetKey<StringSettings>(SettingsOptimization.OptimizeDefault);

	        foreach (var literalExpression in file.Descendants<ICSharpLiteralExpression>()) {
		        CheckString(literalExpression, consumer, stringSettings, _solution, _settingsStore, _daemonProcess);
	        }

            try
            {
                commiter(new DaemonStageResult(consumer.Highlightings));
            }
            catch
            {
                // Do nothing if it doesn't work.
            }
        }


        public static void CheckString(ICSharpLiteralExpression literalExpression,
								DefaultHighlightingConsumer consumer, StringSettings settings, ISolution _solution, IContextBoundSettingsStore _settingsStore, IDaemonProcess _daemonProcess = null)
        {
            //ConstantValue val = literalExpression.ConstantValue;

            // Ignore it unless it's something we're re-evalutating
            if (_daemonProcess != null && !_daemonProcess.IsRangeInvalidated(literalExpression.GetDocumentRange())) return;



            // Ignore verbatim strings.
            if (settings.IgnoreVerbatimStrings &&
                LiteralService.Get(CSharpLanguage.Instance).IsVerbatimStringLiteral(literalExpression)) return;

            ITokenNode tokenNode = literalExpression.Literal;
            if (tokenNode == null) return;

            if (tokenNode.GetTokenType() == CSharpTokenType.STRING_LITERAL_REGULAR)
            {
                ISpellChecker spellChecker = SpellCheckManager.GetSpellChecker(_settingsStore, _solution, settings.DictionaryNames);

                StringSpellChecker.SpellCheck(
                    literalExpression.GetDocumentRange().Document,
                    tokenNode,
                    spellChecker,
                    _solution, consumer, _settingsStore, settings);
            }
        }
        #endregion
    }
}