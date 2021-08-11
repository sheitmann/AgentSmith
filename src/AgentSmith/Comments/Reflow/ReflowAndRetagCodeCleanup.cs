using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeCleanup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Application.Threading;

namespace AgentSmith.Comments.Reflow
{
    [CodeCleanupModule]
    public class ReflowAndRetagCodeCleanup : ICodeCleanupModule
    {
        private readonly IShellLocks _shellLocks;
        private static readonly Descriptor DescriptorInstance = new Descriptor();

        public ReflowAndRetagCodeCleanup(IShellLocks shellLocks)
        {
            _shellLocks = shellLocks;
        }

        public void SetDefaultSetting(CodeCleanupProfile profile, CodeCleanupService.DefaultProfileType profileType) {
            switch (profileType) {
                case CodeCleanupService.DefaultProfileType.FULL:
                    profile.SetSetting(DescriptorInstance, true);
                    break;

                case CodeCleanupService.DefaultProfileType.CODE_STYLE:
                case CodeCleanupService.DefaultProfileType.REFORMAT:
                    profile.SetSetting(DescriptorInstance, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("profileType");
            }
        }

        public bool IsAvailable(IPsiSourceFile sourceFile)
        {
			return sourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance) != null;
        }

        public bool IsAvailable(CodeCleanupProfile profile) {
	        return profile.GetSetting<bool>(DescriptorInstance);
        }

        public string Name => "Reflow & Retag XML Documentation Comments [Agent Smith]";


        public void Process(IPsiSourceFile sourceFile, IRangeMarker rangeMarker, CodeCleanupProfile profile, IProgressIndicator progressIndicator, IUserDataHolder cache)
        {
			IFile file = sourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance);
            if (file == null)
                return;

            if (!profile.GetSetting(DescriptorInstance))
                return;

            file.GetPsiServices().Transactions.Execute("Reflow & Retag XML Documentation Comments",
                () =>
                {
	                using (_shellLocks.UsingWriteLock()) {
		                foreach (var documentBlockOwner in file.Descendants<IDocCommentBlockOwner>()) {
			                CommentReflowAndRetagAction.ReflowAndRetagCommentBlockNode(
				                documentBlockOwner.GetSolution(),
				                null,
				                documentBlockOwner.DocCommentBlock);
		                }
	                };
                });
        }

        public PsiLanguageType LanguageType
        {
            get { return CSharpLanguage.Instance; }
        }

        public ICollection<CodeCleanupOptionDescriptor> Descriptors
        {
            get { return new CodeCleanupOptionDescriptor[] { DescriptorInstance }; }
        }

        public bool IsAvailableOnSelection
        {
            get { return false; }
        }

        [DefaultValue(false)]
        [DisplayName(@"Reflow & Retag XML Documentation Comments")]
        [Category(CSharpCategory)]
        private class Descriptor : CodeCleanupBoolOptionDescriptor
        {
            public Descriptor() : base("AgentSmithReflowAndRetagXMLDoc") { }
        }
    }
}
