using System;
using System.Windows.Forms;

using AgentSmith.Comments;

using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.DocumentModel;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.RichText;
using JetBrains.Util.dataStructures.TypedIntrinsics;
using JetBrains.Util.Logging;

using DataConstants = JetBrains.ProjectModel.DataContext.DataConstants;
using MessageBox = JetBrains.Util.MessageBox;

namespace AgentSmith.SmartPaste
{
	//[ActionHandler(new[] { "AgentSmith.SmartPaste" })]
	[Action("AgentSmith.SmartPaste")]
    internal class SmartInsertAction : IExecutableAction
    {
        #region IActionHandler Members
        
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            bool update = nextUpdate();

            if (!IsAvailable(context))
            {
                return update;
            }
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            if (!IsAvailable(context))
            {
                nextExecute();
            }
            else
            {
                
                ISolution solution = context.GetData(ProjectModelDataConstants.SOLUTION);
                if (solution == null)
                {
                    nextExecute();
                    return;
                }
                solution.GetPsiServices()
	                    .Transactions.Execute("SmartPaste", () => ExecuteEx(solution, context));
            }
        }

        public void ExecuteEx(ISolution solution, IDataContext context)
        {
            ITextControl editor = context.GetData(TextControlDataConstants.TEXT_CONTROL);
            Logger.Assert(editor != null, "Condition (editor != null) is false");

			IDocument document = context.GetData(DocumentModelDataConstants.DOCUMENT);

            if (editor == null || document == null) throw new ArgumentException("context");
			ICSharpFile file = solution.GetPsiServices().GetPsiFile<CSharpLanguage>(new DocumentRange(editor.Document, editor.Caret.Offset())) as ICSharpFile;
            if (file == null) return;
            
            ITreeNode element = file.FindNodeAt(new TreeTextRange(new TreeOffset(editor.Caret.Offset())));
            HandleElement(editor, element, editor.Caret.Offset());

        }

        private static void HandleElement(ITextControl editor, ITreeNode element, int offset)
        {
            string stringToInsert = Clipboard.GetText();
            if (string.IsNullOrEmpty(stringToInsert))
            {
                return;
            }

            IDocCommentNode docCommentNode = element as IDocCommentNode;
            if (docCommentNode != null)
            {
                Int32<DocLine> currentLineNumber =
                    editor.Document.GetCoordsByOffset(editor.Caret.Offset()).Line;
                string currentLine = editor.Document.GetLineText(currentLineNumber);
                int index = currentLine.IndexOf("///", StringComparison.Ordinal);
                if (index < 0)
                {
                    return;
                }
                string prefix = currentLine.Substring(0, index);

                if (ShallEscape(docCommentNode, editor.Caret.Offset()) &&
                    RichTextBlockToHtml.HtmlEncode(stringToInsert) != stringToInsert &&
                    MessageBox.ShowYesNo("Do you want the text to be escaped?"))
                {
                    stringToInsert = RichTextBlockToHtml.HtmlEncode(stringToInsert);
                }

                stringToInsert = stringToInsert.Replace("\n", "\n" + prefix + "///");
            }

            ITokenNode token = element as ITokenNode;
            if (token != null)
            {
                if ((token.GetTokenType() == CSharpTokenType.STRING_LITERAL_REGULAR
					|| token.GetTokenType() == CSharpTokenType.STRING_LITERAL_VERBATIM) 
					&& offset < token.GetTreeTextRange().EndOffset.Offset)
                {
                    string text = token.GetText();
                    if (text.StartsWith("@") && offset > token.GetTreeTextRange().StartOffset.Offset + 1)
                    {
                        stringToInsert = stringToInsert.Replace("\"", "\"\"");
                    }
                    else if (!text.StartsWith("@"))
                    {
                        stringToInsert = stringToInsert.
                            Replace("\\", "\\\\").
                            Replace("\a", "\\a").
                            Replace("\b", "\\b").
                            Replace("\f", "\\f").
                            Replace("\n", "\\n").
                            Replace("\r", "\\r").
                            Replace("\t", "\\t").
                            Replace("\v", "\\v").
                            Replace("\'", "\\'").
                            Replace("\"", "\\\"");
                    }
                }
            }

            editor.Document.InsertText(editor.Caret.Offset(), stringToInsert);
        }

        private static bool ShallEscape(IDocCommentNode node, int offset)
        {
            IDocCommentBlock docBlock = node.GetContainingNode<IDocCommentBlock>(true);
            if (docBlock == null)
            {
                return false;
            }
            XmlDocLexer lexer = new XmlDocLexer(docBlock);
            lexer.Start();

            bool inCData = false;
            bool insideTag = false;
            while (lexer.TokenType != null)
            {
                if (lexer.TokenType == lexer.XmlTokenType.TAG_START)
                {
                    insideTag = true;
                }
                else if (lexer.TokenType == lexer.XmlTokenType.TAG_END)
                {
                    insideTag = false;
                }
                else if (lexer.TokenType == lexer.XmlTokenType.TAG_START1)
                {
                    insideTag = true;
                }
                else if (lexer.TokenType == lexer.XmlTokenType.TAG_END1)
                {
                    insideTag = false;
                }
                else if (lexer.TokenType == lexer.XmlTokenType.CDATA_START)
                {
                    inCData = true;
                }
                else if (lexer.TokenType == lexer.XmlTokenType.CDATA_END)
                {
                    inCData = false;
                }
                else if (offset >= lexer.TokenLocalRange.StartOffset && offset <= lexer.TokenLocalRange.EndOffset)
                {
                    return !inCData && !insideTag;
                }
                else if (offset < lexer.TokenLocalRange.StartOffset)
                {
                    return false;
                }
                lexer.Advance();
            }

            return false;
        }

        #endregion

        private static bool IsAvailable(IDataContext context)
        {
            ISolution solution = context.GetData(ProjectModelDataConstants.SOLUTION);
			IDocument document = context.GetData(DocumentModelDataConstants.DOCUMENT);
            IPsiSourceFile file = null;
            if (solution != null && document != null) file = document.GetPsiSourceFile(solution);
            return solution != null && document != null && file != null && file.PrimaryPsiLanguage.Is<CSharpLanguage>() ;
        }
    }
}