using System;
using System.Collections.Generic;
using System.Xml;

using AgentSmith.Comments;
using AgentSmith.Identifiers;
using AgentSmith.Options;

using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentSmith
{
    /// <summary>
    /// Process stage for analysing comments in a project file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// We scan for all declarations.
    /// </para>
    /// <para>
    /// For each declaration we do several checks:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// Does the member have an XML documentation comment?
    /// </item>
    /// <item>
    /// Are all the words in the comment correctly spelled?
    /// </item>
    /// <item>
    /// Are there any words that look like identifiers that we should wrap with meta-tags?
    /// </item>
    /// </list>
    /// </remarks>
    internal class IdentifierScanDaemonStageProcess : IDaemonStageProcess
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
        public IdentifierScanDaemonStageProcess(IDaemonProcess daemonProcess, IContextBoundSettingsStore settingsStore)
        {
            _daemonProcess = daemonProcess;
            _solution = daemonProcess.Solution;

            _settingsStore = settingsStore;


        }

        #region IDaemonStageProcess Members

        /// <summary>
        /// The current instance process that this stage is a part of.
        /// </summary>
        public IDaemonProcess DaemonProcess { get { return _daemonProcess; } }



        private void CheckMember(IClassMemberDeclaration declaration,
                                                DefaultHighlightingConsumer consumer, CommentAnalyzer commentAnalyzer, IdentifierSpellCheckAnalyzer identifierAnalyzer)
        {
            if (declaration is IConstructorDeclaration && declaration.IsStatic)
            {
                // TODO: probably need to put this somewhere in settings.
                //Static constructors have no visibility so not clear how to check them.
                return;
            }


            // Documentation doesn't work properly on multiple declarations (as of R# 6.1) so see if we can get it from the parent
            XmlNode docNode = null;
            IDocCommentBlock commentBlock;
            IMultipleDeclarationMember multipleDeclarationMember = declaration as IMultipleDeclarationMember;
            if (multipleDeclarationMember != null)
            {
                // get the parent
                IMultipleDeclaration multipleDeclaration = multipleDeclarationMember.MultipleDeclaration;

                // Now ask for the actual comment block
                commentBlock = SharedImplUtil.GetDocCommentBlockNode(multipleDeclaration);

                if (commentBlock != null) docNode = commentBlock.GetXML(null);
            }
            else
            {
                commentBlock = SharedImplUtil.GetDocCommentBlockNode(declaration);

                docNode = declaration.GetXMLDoc(false);

            }

            commentAnalyzer.CheckMemberHasComment(declaration, docNode, consumer);
            commentAnalyzer.CheckCommentSpelling(declaration, commentBlock, consumer, true);
            identifierAnalyzer.CheckMemberSpelling(declaration, consumer, true);
        }

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

            StringSettings stringSettings = _settingsStore.GetKey<StringSettings>(SettingsOptimization.OptimizeDefault);


            //if (!_daemonProcess.FullRehighlightingRequired) return;

            CommentAnalyzer commentAnalyzer = new CommentAnalyzer(_solution, _settingsStore);
            IdentifierSpellCheckAnalyzer identifierAnalyzer = new IdentifierSpellCheckAnalyzer(_solution, _settingsStore, _daemonProcess.SourceFile);

#if RESHARPER20173
	        var consumer = new DefaultHighlightingConsumer(_daemonProcess.SourceFile);
#else
			var consumer = new DefaultHighlightingConsumer(this, _settingsStore);  
#endif
			
			foreach (var classMemberDeclaration in file.Descendants<IClassMemberDeclaration>()) {
		        CheckMember(classMemberDeclaration, consumer, commentAnalyzer, identifierAnalyzer);
	        }

	        if (_daemonProcess.InterruptFlag) return;
            try
            {
                commiter(new DaemonStageResult(consumer.Highlightings));
            } catch
            {
                // Do nothing if it doesn't work.
            }
        }
        #endregion
    }
}