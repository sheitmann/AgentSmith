using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace AgentSmith.Comments
{
    /// <summary>
    /// Quick fix for members without XML documentation comments which will add a new blank comment.
    /// </summary>
    [QuickFix]
    public class AddXmlCommentQuickFix : IQuickFix
    {
        /// <summary>
        /// The member declaration we're associated with.
        /// </summary>
        private readonly IDeclaration _declaration;

        /// <summary>
        /// Create a new quick fix for the given highlight
        /// </summary>
        /// <param name="highlight">The highlight which identifies the declaration without a comment</param>
        public AddXmlCommentQuickFix(MissingXmlCommentHighlight highlight)
        {
            _declaration = highlight.Declaration;
        }

        #region IQuickFix Members

	    public IEnumerable<IntentionAction> CreateBulbItems() {
			return new AddCommentBulbItem(_declaration).ToContextActionIntentions();
		}

	    /// <summary>
        /// Check if this action is available at the constructed context.
        ///             Actions could store pre-calculated info in <paramref name="cache"/> to share it between different actions
        /// </summary>
        /// <returns>
        /// true if this bulb action is available, false otherwise.
        /// </returns>
        public bool IsAvailable(IUserDataHolder cache)
        {
            return true;
        }

#endregion
    }

    /// <summary>
    /// Bulb item which allows the user to add a new, empty XML documentation comment to a declaration.
    /// </summary>
    internal class AddCommentBulbItem : IBulbAction
    {
        /// <summary>
        /// Internal storage for the declaration to add the comment to
        /// </summary>
        private readonly IDeclaration _declaration;

        /// <summary>
        /// Create a new bulb item
        /// </summary>
        /// <param name="declaration">The declaration to add the comment to</param>
        public AddCommentBulbItem(IDeclaration declaration)
        {
            _declaration = declaration;
        }

#region IBulbItem Members

        /// <summary>
        /// Invoked when the user executes this bulb item.
        /// </summary>
        /// <param name="solution">The currently open solution.</param>
        /// <param name="textControl">The text control that is currently open</param>
        public void Execute(ISolution solution, ITextControl textControl)
        {
            if (solution.GetPsiServices().Caches.WaitForCaches("Add Comment Bulb Item"))
            {
                solution.GetPsiServices().Transactions.Execute(GetType().Name, () => ExecuteEx(solution, textControl));
            }
        }

        /// <summary>
        /// The text rendered directly on the bulb item.
        /// </summary>
        public string Text
        {
            get { return "Add comment stub."; }
        }

#endregion

        /// <summary>
        /// Our internal implementation of the bulb item which assumes it is running within a transaction
        /// </summary>
        /// <param name="solution">The currently open solution</param>
        /// <param name="textControl">The text control that is currently open</param>
        public void ExecuteEx(ISolution solution, ITextControl textControl)
        {
            // Get the comment block owner (ie the part of the declaration which will own the comment).
            IDocCommentBlockOwner docCommentBlockOwnerNode =
                XmlDocTemplateUtil.FindDocCommentOwner(_declaration as ITypeMemberDeclaration);

            // If we didn't get an owner then give up
            if (docCommentBlockOwnerNode == null) return;
            
            // We got one

            // Ask resharper to create the xml for us.
            int myCursorOffset;
            string text = XmlDocTemplateUtil.GetDocTemplate(docCommentBlockOwnerNode, out myCursorOffset);

            // Get a factory which can create elements in the C# docs
            CSharpElementFactory factory = CSharpElementFactory.GetInstance(docCommentBlockOwnerNode.GetPsiModule());

            // Create the comment block
            IDocCommentBlock comment = factory.CreateDocCommentBlock(text);

            // And set the comment on the declaration.
            docCommentBlockOwnerNode.SetDocCommentBlock(comment);
        }
    }
}