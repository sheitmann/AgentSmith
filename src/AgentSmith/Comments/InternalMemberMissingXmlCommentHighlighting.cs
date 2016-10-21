using AgentSmith.Comments;
using AgentSmith.MemberMatch;

using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

#if RESHARPER20161 || RESHARPER20162
[assembly: RegisterConfigurableSeverity(
    InternalMemberMissingXmlCommentHighlighting.SEVERITY_ID,
    null,
    HighlightingGroupIds.CodeSmell,
    "Internal members should have XML documentation",
    "Internal members should have XML documentation",
    Severity.WARNING,
    false)]
#else
[assembly: RegisterConfigurableSeverity(
    InternalMemberMissingXmlCommentHighlighting.SEVERITY_ID,
    null,
    HighlightingGroupIds.CodeSmell,
    "Internal members should have XML documentation",
    "Internal members should have XML documentation",
    Severity.WARNING)]
#endif

namespace AgentSmith.Comments
{
    /// <summary>
    /// A highlight for internal members which don't have xml documentation comments
    /// </summary>
    [ConfigurableSeverityHighlighting(SEVERITY_ID, CSharpLanguage.Name)]
    public class InternalMemberMissingXmlCommentHighlighting : MissingXmlCommentHighlight
    {

        /// <summary>
        /// Create a new highlight
        /// </summary>
        /// <param name="declaration">The declaration which is missing the comment</param>
        /// <param name="match">The match object that was used to compare the access levels and etc.</param>
        public InternalMemberMissingXmlCommentHighlighting(IDeclaration declaration, Match match)
            : base(declaration, match)
        {
        }

        /// <summary>
        /// The ID of this highlight - used to uniquely identify this highlight in the options.
        /// </summary>
		public const string SEVERITY_ID = "InternalMembersMustHaveComments";

    }
}