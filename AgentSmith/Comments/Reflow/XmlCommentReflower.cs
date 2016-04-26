using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using AgentSmith.Comments.Reflow.XmlComments;
using AgentSmith.Options;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith.Comments.Reflow
{
    /// <remarks>
    /// 1. All whitespace after a tag is preserved.
    /// 2. <c></c> and <code></code> are treated as non reflowable tags.
    /// 3. Blank lines or (?)are treated as paragraph separators.
    /// 4. Line with indentation different from previous line is paragraph start.
    /// 4. XML tags are not split.
    /// 5. Bullet points are paragraph start.
    /// 6. Non reflowable expressions?
    /// </remarks>
    public class XmlCommentReflower
    {

        private readonly Regex _bulletItemRegex = new Regex(@"^\s*[-*]\s+(.*)$");
        private readonly Regex _numberItemRegex = new Regex(@"^\s*\d+\.?\s+(.*)$");
        private readonly Regex _paramRegex = new Regex(@"^\s*<\s*(?:type)?param\s+name\s*=\s*""([_a-zA-Z0-9]+)""");
        private readonly Regex _endParamRegex = new Regex(@"^\s*</\s*(?:type)?param\s*>");

        private readonly XmlDocumentationSettings _settings;
        private readonly ReflowAndRetagSettings _reflowSettings;

        public XmlCommentReflower(XmlDocumentationSettings settings, ReflowAndRetagSettings reflowAndRetagSettings)
        {
            _settings = settings;
            _reflowSettings = reflowAndRetagSettings;
        }

        public IEnumerable<Paragraph> Parse(IDocCommentBlock blockNode)
        {
            // Create a lexer which can read the comment
            XmlCommentReflowableBlockLexer lexer = new XmlCommentReflowableBlockLexer(blockNode);

            // Create a parser which can turn the comment into paragraphs and lines
            XmlCommentParagraphParser paragraphParser = new XmlCommentParagraphParser(lexer);

            // Firstly walk through the paragraphs and:
            // - collapse each one to the minimum number of required lines (tags always take a whole line at this stage).
            // - remove extra whitespace. There's no point in having it, it wont show in generated doco
            return paragraphParser.Parse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockNode"></param>
        /// <param name="maxLineLength"></param>
        /// <returns></returns>
        public string Reflow(IDocCommentBlock blockNode, int maxLineLength)
        {
            return ReflowToLineLength(Parse(blockNode), maxLineLength);
        }

        public string ReflowToLineLength(IEnumerable<Paragraph> paragraphs, int maxLineLength)
        {
            LineBuilder lb = new LineBuilder();

            bool firstParagraph = true;
            foreach (Paragraph paragraph in paragraphs)
            {
                if (!firstParagraph)
                {
                    lb.Append("\r\n");
                }

                ParagraphLineItem previousItem = null;
                foreach (ParagraphLine paragraphLine in paragraph.Lines)
                {
                    foreach (ParagraphLineItem lineItem in paragraphLine.Items)
                    {
                        if (lineItem.ItemType == ItemType.XmlElement ||
                            lineItem.ItemType == ItemType.NonReflowableBlock)
                        {
                            if ( //if current line is empty, no matter how big text is, just append it (to 
                                // not create extra lines.
                                lb.CurrentLine.Trim().Length > 0 &&
                                //Append new line otherwise
                                lb.CurrentLine.Length + lineItem.FirstLine.Length > maxLineLength)
                            {
                                lb.AppendMultilineBlock("\r\n" + paragraph.Offset);
                            }
                            lb.AppendMultilineBlock(lineItem.Text);
                        }

                       //Space between XML elements is not reflown.
                        else if (lineItem.ItemType == ItemType.XmlSpace)
                        {
                            if (previousItem != null && previousItem.IsForcingNewLine)
                            {
                                // do not create new line if it is the last item
                                // in the paragraph
                                if ((lineItem != paragraphLine.Items[paragraphLine.Items.Count - 1])
                                    || (paragraphLine != paragraph.Lines[paragraph.Lines.Count - 1]))
                                {
                                    lb.AppendMultilineBlock(
                                        "\r\n" + paragraph.Offset);
                                }
                            }
                            else
                            {
                                lb.AppendMultilineBlock(lineItem.Text);
                            }
                        }

                        else if (lineItem.ItemType == ItemType.Text)
                        {
                            string text = lineItem.Text;
                            if (lineItem == paragraphLine.Items[0])
                            {
                                text = text.TrimStart();
                            }

                            if (previousItem != null && previousItem.IsForcingNewLine)
                            {
                                lb.AppendMultilineBlock("\r\n" + paragraph.Offset);
                                text = text.TrimStart();
                            }

                            string[] words = text.Split(' ');

                            for (int i = 0; i < words.Length; i++)
                            {
                                // append the space at the start of the line
                                if (lb.CurrentLine.Length == 0)
                                {
                                    lb.AppendMultilineBlock(paragraph.Offset);
                                }

                                string word = words[i];

                                if (lb.CurrentLine.Length == paragraph.Offset.Length && word.Trim().Length == 0)
                                {
                                    continue;
                                }

                                //prepend space if this is not first word in block and not first word on paragraph line 
                                //or this is first word in block and block is appended to previous line.
                                bool previousBlockIsText = previousItem != null && previousItem.ItemType == ItemType.Text;
                                string toAppend = (lb.CurrentLine.Length > paragraph.Offset.Length && (i > 0 || previousBlockIsText) ? " " : "") + word;
                                if (lb.CurrentLine.Length + toAppend.Length > maxLineLength)
                                {
                                    lb.AppendMultilineBlock("\r\n" + paragraph.Offset);
                                    toAppend = word;
                                }

                                lb.AppendMultilineBlock(toAppend);
                            }
                        }
                        previousItem = lineItem;
                    }
                }
                firstParagraph = false;
            }

            return lb.ToString();
        }


        /// <summary>
        /// Reflow the given comment block to fit within the given maximum line length
        /// </summary>
        /// <param name="blockNode">The comment block to reflow</param>
        /// <param name="maxLineLength">The maximum line length</param>
        /// <returns>The text for the new reflown comment.</returns>
        public string ReflowAndRetag(IDocCommentBlock blockNode, int maxLineLength)
        {
            ITreeNode parent = blockNode.Parent;
            ICSharpTypeMemberDeclaration parentDeclaration = parent as IClassMemberDeclaration;
            if (parentDeclaration == null)
            {
                IMultipleFieldDeclaration multipleFieldDeclaration = parent as IMultipleFieldDeclaration;
                if (multipleFieldDeclaration != null)
                {
                    foreach (IFieldDeclaration field in multipleFieldDeclaration.Children<IFieldDeclaration>())
                    {
                        parentDeclaration = field;
                        break;
                    }
                }

                IEnumMemberDeclaration enumMemberDeclaration = parent as IEnumMemberDeclaration;
                if (enumMemberDeclaration != null)
                {
                    parentDeclaration = enumMemberDeclaration;
                }

            }

            // get the xml from the comment
            XmlNode node = blockNode.GetXML(null);

            // Walk the xml tree and process elements as we go. Use a recursive algo for now - comments shouldn't be that complex.
            XmlCommentOptions options = new XmlCommentOptions();
            options.Declaration = parentDeclaration;
            options.IdentifierLookupScope = IdentifierLookupScopes.ProjectAndUsings;
            options.Solution = blockNode.GetSolution();

            List<Regex> ignoreList = new List<Regex>(_settings.CompiledWordsToIgnoreForMetatagging);
            ignoreList.Add(new Regex("^[Aa]$"));
            ignoreList.Add(new Regex("^[iI]f$"));
            ignoreList.Add(new Regex("^[tT]his$"));
            ignoreList.Add(new Regex("^[eE]lse$"));
            ignoreList.Add(new Regex("^[lL]ong$"));
            ignoreList.Add(new Regex("^[wW]hile$"));
            ignoreList.Add(new Regex("^[lL]ock$"));
            ignoreList.Add(new Regex("^[fF]ixed$"));
            ignoreList.Add(new Regex("^[bB]ase$"));
            ignoreList.Add(new Regex("^[oO]bject$"));

            options.IdentifiersToIgnoreForMetaTagging = ignoreList;

            options.Settings = _reflowSettings;

            XmlComments.XmlComment comment = new XmlComments.XmlComment(options);
            comment.FromXml(node);
            comment.InsertMissingTags();
            return comment.ToXml(0,maxLineLength, 0);
        }
        


    }
}