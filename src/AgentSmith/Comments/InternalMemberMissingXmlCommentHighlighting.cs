using AgentSmith.MemberMatch;

using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith.Comments {
	/// <summary>
	/// A highlight for internal members which don't have xml documentation comments
	/// </summary>
	/// 
    [RegisterConfigurableSeverity(
		SEVERITY_ID,
		null,
		HighlightingGroupIds.CodeSmell,
		"Internal members should have XML documentation",
		"Internal members should have XML documentation",
		Severity.WARNING)]
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
		private const string SEVERITY_ID = "InternalMembersMustHaveComments";

    }
}