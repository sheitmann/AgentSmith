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

#if RESHARPER20172
using JetBrains.Application.Threading;
#endif

namespace AgentSmith.Comments.Reflow
{
    [CodeCleanupModule]
    public class ReflowCodeCleanup : ICodeCleanupModule
    {
        private readonly IShellLocks _shellLocks;
        private static readonly Descriptor DescriptorInstance = new Descriptor();

        public ReflowCodeCleanup(IShellLocks shellLocks)
        {
            _shellLocks = shellLocks;
        }

        public void SetDefaultSetting(CodeCleanupProfile profile, CodeCleanup.DefaultProfileType profileType)
        {
            switch (profileType)
            {
                case CodeCleanup.DefaultProfileType.FULL:
                    profile.SetSetting(DescriptorInstance, false);
                    break;
#if RESHARPER20191
                case CodeCleanup.DefaultProfileType.CODE_STYLE:
#endif
                case CodeCleanup.DefaultProfileType.REFORMAT:
                    profile.SetSetting(DescriptorInstance, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("profileType");
            }
        }

        public bool IsAvailable(IPsiSourceFile sourceFile)
        {
			return sourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance) != null;
        }

        public void Process(IPsiSourceFile sourceFile, IRangeMarker rangeMarker, CodeCleanupProfile profile, IProgressIndicator progressIndicator)
        {
			var file = sourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance);
            if (file == null)
                return;

            if (!profile.GetSetting(DescriptorInstance))
                return;

            file.GetPsiServices().Transactions.Execute("Reflow XML Documentation Comments",
                () =>
                {
	                using (_shellLocks.UsingWriteLock()) {
		                foreach (var docCommentBlock in file.Descendants<IDocCommentBlock>()) {
			                CommentReflowAction.ReFlowCommentBlockNode(file.GetSolution(), progressIndicator, docCommentBlock);
		                }
	                }
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
        [DisplayName(@"Reflow XML Documentation Comments")]
        [Category(CSharpCategory)]
        private class Descriptor : CodeCleanupBoolOptionDescriptor
        {
            public Descriptor() : base("AgentSmithReflowXMLDoc") { }
        }
    }
}
