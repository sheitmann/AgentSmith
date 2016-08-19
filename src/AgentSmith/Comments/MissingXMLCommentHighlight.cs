using System;
using AgentSmith.MemberMatch;

using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith.Comments
{
    /// <summary>
    /// Base class for all highlights related to missing XML documentation comments.
    /// </summary>
    public abstract class MissingXmlCommentHighlight : IHighlighting
    {
        /// <summary>
        /// Internal storage for the declaration which is missing the comment.
        /// </summary>
        private readonly IDeclaration _declaration;

        /// <summary>
        /// Internal storage for the match object that was used to compare the access levels and etc.
        /// </summary>
        private readonly Match _match;

        /// <summary>
        /// Create a new highlight
        /// </summary>
        /// <param name="declaration">The declaration which is missing the comment</param>
        /// <param name="match">The match object that was used to compare the access levels and etc.</param>
        protected MissingXmlCommentHighlight(IDeclaration declaration, Match match)
        {
            _declaration = declaration;
            _match = match;
            if (_match == null)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// The declaration which is missing the comment
        /// </summary>
        public IDeclaration Declaration { get { return _declaration; } }

        /// <summary>
        /// The match object that was used to compare the access levels and etc
        /// </summary>
        public Match Match { get { return _match; } }

	    /// <summary>
	    /// Calculates range of a highlighting.
	    /// </summary>
	    public DocumentRange CalculateRange() {
		    return _declaration.GetDocumentRange();
	    }

	    /// <summary>
        /// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlightingAttributeBase.ShowToolTipInStatusBar"/> is <c>true</c>)
        ///             To override the default mechanism of tooltip, mark the implementation class with 
        ///             <see cref="T:JetBrains.ReSharper.Daemon.DaemonTooltipProviderAttribute"/> attribute, and then this property will not be called
        /// </summary>
        public string ToolTip
        {
            get { return Match + "should have XML comment."; }
        }

        /// <summary>
        /// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlightingAttributeBase.ShowToolTipInStatusBar"/> is <c>true</c>)
        /// </summary>
        public string ErrorStripeToolTip
        {
            get { return ToolTip; }
        }

        /// <summary>
        /// Specifies the offset from the Range.StartOffset to set the cursor to when navigating 
        ///             to this highlighting. Usually returns <c>0</c>
        /// </summary>
        public int NavigationOffsetPatch
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns true if data (PSI, text ranges) associated with highlighting is valid
        /// </summary>
        public bool IsValid()
        {
            return Match != null;
        }
    }
}