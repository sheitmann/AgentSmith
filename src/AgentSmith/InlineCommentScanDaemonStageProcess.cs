using System;
using System.Collections.Generic;

using AgentSmith.Comments;
using AgentSmith.Options;
using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;
using AgentSmith.Strings;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentSmith
{
    public class InlineCommentScanDaemonStageProcess : IDaemonStageProcess
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
        public InlineCommentScanDaemonStageProcess(IDaemonProcess daemonProcess, IContextBoundSettingsStore settingsStore)
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
            IFile file = _daemonProcess.SourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance);
            if (file == null)
            {
                return;
            }
			var consumer = new DefaultHighlightingConsumer(this, _settingsStore);       
            var commentSettings = _settingsStore.GetKey<CommentSettings>(SettingsOptimization.OptimizeDefault);

	        foreach (var commentNode in file.Descendants<ICSharpCommentNode>()) {
		        CheckComment(commentNode, consumer, commentSettings);
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


        public void CheckComment(ICSharpCommentNode commentNode,
                                DefaultHighlightingConsumer consumer, CommentSettings settings)
        {
            // Ignore it unless it's something we're re-evalutating
            if (!_daemonProcess.IsRangeInvalidated(commentNode.GetDocumentRange())) return;

            // Only look for ones that are not doc comments
            if (commentNode.CommentType != CommentType.END_OF_LINE_COMMENT &&
                commentNode.CommentType != CommentType.MULTILINE_COMMENT) return;

            ISpellChecker spellChecker = SpellCheckManager.GetSpellChecker(_settingsStore, _solution, settings.DictionaryNames);

            SpellCheck(
                commentNode.GetDocumentRange().Document,
                commentNode,
                spellChecker,
                _solution, consumer, _settingsStore, settings);
            
        }

        public static void SpellCheck(IDocument document, ITokenNode token, ISpellChecker spellChecker,
                                               ISolution solution, DefaultHighlightingConsumer consumer, IContextBoundSettingsStore settingsStore, CommentSettings settings)
        {
            if (spellChecker == null) return;

            string buffer = token.GetText();
            ILexer wordLexer = new WordLexer(buffer);
            wordLexer.Start();
            while (wordLexer.TokenType != null)
            {
                string tokenText = wordLexer.GetCurrTokenText();
                if (SpellCheckUtil.ShouldSpellCheck(tokenText, settings.CompiledWordsToIgnore) &&
                    !spellChecker.TestWord(tokenText, true))
                {
                    IClassMemberDeclaration containingElement =
                        token.GetContainingNode<IClassMemberDeclaration>(false);
                    if (containingElement == null ||
                        !IdentifierResolver.IsIdentifier(containingElement, solution, tokenText))
                    {
                        CamelHumpLexer camelHumpLexer = new CamelHumpLexer(buffer, wordLexer.TokenStart, wordLexer.TokenEnd);
                        foreach (LexerToken humpToken in camelHumpLexer)
                        {
                            if (SpellCheckUtil.ShouldSpellCheck(humpToken.Value, settings.CompiledWordsToIgnore) &&
                                !spellChecker.TestWord(humpToken.Value, true))
                            {
								//int start = token.GetTreeStartOffset().Offset + wordLexer.TokenStart;
								//int end = start + tokenText.Length;

								//var range = new TextRange(start, end);
								//var documentRange = new DocumentRange(document, range);
	                            DocumentRange documentRange =
		                            token.GetContainingFile().TranslateRangeForHighlighting(token.GetTreeTextRange());
								documentRange = documentRange.ExtendLeft(-wordLexer.TokenStart);
								documentRange = documentRange.ExtendRight(-1*(documentRange.GetText().Length - tokenText.Length));


                                TextRange textRange = new TextRange(humpToken.Start - wordLexer.TokenStart,
                                    humpToken.End - wordLexer.TokenStart);
	                            //string word = document.GetText(range);
	                            string word = documentRange.GetText();
	                            consumer.AddHighlighting(
		                            new StringSpellCheckHighlighting(
			                            word,
			                            documentRange,
			                            humpToken.Value,
			                            textRange,
			                            solution,
			                            spellChecker,
			                            settingsStore),
		                            documentRange);
                                break;
                            }
                        }
                    }
                }

                wordLexer.Advance();
            }
        }

        #endregion
    }
}