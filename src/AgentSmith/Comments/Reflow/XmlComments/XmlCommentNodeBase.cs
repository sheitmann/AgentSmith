using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using AgentSmith.Options;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Caches;

namespace AgentSmith.Comments.Reflow.XmlComments
{

    public class XmlCommentOptions
    {
        public ICSharpTypeMemberDeclaration Declaration;

        public ISolution Solution;

        public IdentifierLookupScopes IdentifierLookupScope;

        public IEnumerable<Regex> IdentifiersToIgnoreForMetaTagging;

        public ReflowAndRetagSettings Settings;

    }

    public interface IXmlCommentNode
    {
        void FromXml(XmlNode node);

        void InsertMissingTags();

        string ToXml(int currentLineLength, int maxLineLength, int indent);
    }

    public abstract class XmlCommentNodeBase : IXmlCommentNode
    {
        public XmlCommentOptions Options { get; private set; }

        protected XmlCommentNodeBase(XmlCommentOptions options)
        {
            Options = options;
        }

        public abstract void FromXml(XmlNode node);
        public abstract void InsertMissingTags();
        public abstract string ToXml(int currentLineLength, int maxLineLength, int indent);
    }

    public class XmlComment : XmlCommentNodeBase
    {
        public SummaryNode Summary { get; set; }
        public RemarksNode Remarks { get; set; }
        public ValueNode Value { get; set; }
        public List<ExampleNode> Examples { get; set; }
        public List<TypeParamNode> TypeParams { get; set; }
        public List<ParamNode> Params { get; set; }
        public List<ExceptionNode> Exceptions { get; set; }
        public List<PermissionNode> Permissions { get; set; }
        public ReturnsNode Returns { get; set; }
        public List<UnknownTagNode> UnknownTagNodes { get; set; }

        public XmlComment(XmlCommentOptions options) : base(options)
        {
            Examples = new List<ExampleNode>();
            TypeParams = new List<TypeParamNode>();
            Params = new List<ParamNode>();
            Exceptions = new List<ExceptionNode>();
            Permissions = new List<PermissionNode>();
            UnknownTagNodes = new List<UnknownTagNode>();
        }
        public override void FromXml(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                XmlElement element = child as XmlElement;
                if (element != null)
                {
                    switch (element.Name.ToLower())
                    {
                        case "summary":
                            if (Summary == null) Summary = new SummaryNode(Options);
                            Summary.FromXml(element);
                            break;
                        case "remark":
                        case "remarks":
                            if (Remarks == null) Remarks = new RemarksNode(Options);
                            Remarks.FromXml(element);
                            break;
                        case "value":
                            if (Value == null) Value = new ValueNode(Options);
                            Value.FromXml(element);
                            break;
                        case "example":
                            ExampleNode example = new ExampleNode(Options);
                            example.FromXml(element);
                            Examples.Add(example);
                            break;
                        case "typeparam":
                            TypeParamNode typeParam = new TypeParamNode(Options);
                            typeParam.FromXml(element);
                            TypeParams.Add(typeParam);
                            break;
                        case "param":
                            ParamNode param = new ParamNode(Options);
                            param.FromXml(element);
                            Params.Add(param);
                            break;
                        case "exception":
                            ExceptionNode exception = new ExceptionNode(Options);
                            exception.FromXml(element);
                            Exceptions.Add(exception);
                            break;
                        case "permissions":
                            PermissionNode permission = new PermissionNode(Options);
                            permission.FromXml(element);
                            Permissions.Add(permission);
                            break;
                        case "result":
                        case "return":
                        case "returns":
                            if (Returns == null) Returns = new ReturnsNode(Options);
                            Returns.FromXml(element);
                            break;
                        default:
                            // If we see another tag then they should have escaped it but for now just maintain it
                            string outerXml = element.OuterXml;
                            string startTag = outerXml.Substring(0, outerXml.IndexOf('>') + 1).Trim();
                            //startTag = startTag.Replace("<", "&lt;").Replace(">", "&gt;");
                            string endTag = element.IsEmpty? "" : string.Format("</{0}>", element.LocalName);
                            UnknownTagNode unknown = new UnknownTagNode(Options) { StartTag = startTag, EndTag = endTag };
                            unknown.FromXml(element);
                            UnknownTagNodes.Add(unknown);
                            break;
                    }
                    continue;
                }

                // Not an element - dunno what to do with it
            }
        }

        public override void InsertMissingTags()
        {
            if (Summary != null) Summary.InsertMissingTags();
            if (Remarks != null) Remarks.InsertMissingTags();
            if (Value != null) Value.InsertMissingTags();
            foreach (var child in Examples) child.InsertMissingTags();
            foreach (var child in TypeParams) child.InsertMissingTags();
            foreach (var child in Params) child.InsertMissingTags();
            foreach (var child in Exceptions) child.InsertMissingTags();
            foreach (var child in Permissions) child.InsertMissingTags();
            if (Returns != null) Returns.InsertMissingTags();
        }

        public override string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            StringBuilder sb = new StringBuilder();

            indent = 0;

            if (Summary != null)
            {
                sb.Append(Summary.ToXml(0, maxLineLength, indent));
                sb.Append("\n");
            }
            if (Remarks != null)
            {
                sb.Append(Remarks.ToXml(0, maxLineLength, indent));
                sb.Append("\n");
            }
            if (Value != null)
            {
                sb.Append(Value.ToXml(0, maxLineLength, indent));
                sb.Append("\n");
            }
            if (Examples.Count > 0)
            {
                foreach (var child in Examples)
                {
                    sb.Append(child.ToXml(0, maxLineLength, indent));
                    sb.Append("\n");
                }
            }
            if (TypeParams.Count > 0)
            {
                foreach (var child in TypeParams)
                {
                    sb.Append(child.ToXml(0, maxLineLength, indent));
                    sb.Append("\n");
                }
            }
            if (Params.Count > 0)
            {
                foreach (var child in Params)
                {
                    sb.Append(child.ToXml(0, maxLineLength, indent));
                    sb.Append("\n");
                }
            }
            if (Exceptions.Count > 0)
            {
                foreach (var child in Exceptions)
                {
                    sb.Append(child.ToXml(0, maxLineLength, indent));
                    sb.Append("\n");
                }
            }
            if (Permissions.Count > 0)
            {
                foreach (var child in Permissions)
                {
                    sb.Append(child.ToXml(0, maxLineLength, indent));
                    sb.Append("\n");
                }
            }
            if (Returns != null)
            {
                sb.Append(Returns.ToXml(0, maxLineLength, indent));
                sb.Append("\n");
            }
            if (UnknownTagNodes.Count > 0)
            {
                foreach (var child in UnknownTagNodes)
                {
                    sb.Append(child.ToXml(0, maxLineLength, indent));
                    sb.Append("\n");
                }
            }
            string result = sb.ToString();
            if (result.EndsWith("\n")) result = result.Substring(0, result.Length - 1);

            return result;
        }
    }

    public abstract class ExtendedBlockNode : XmlCommentNodeBase
    {
        private readonly Regex _bulletItemRegex = new Regex(@"^[-*]$");
        private readonly Regex _numberItemRegex = new Regex(@"^\d+\.?$");
        private readonly Regex _whitespaceRegex = new Regex(@"\s+");
        private readonly Regex _endsWithNewline = new Regex("\n\\s*$");

        public List<IXmlCommentNode> Contents { get; set; }

        protected ExtendedBlockNode(XmlCommentOptions options) : base(options)
        {
            Contents = new List<IXmlCommentNode>();
        }

        public abstract string GetStartTag();
        public abstract string GetEndTag();

        public virtual WhitespaceTriState NewlineSetting { get { return WhitespaceTriState.Always; } }
        public virtual bool IndentSetting { get { return false; } }

        public virtual int IndentSize { get { return 4; } }

        public virtual IEnumerable<Regex> IdentifiersToIgnore
        {
            get
            {
                return Options.IdentifiersToIgnoreForMetaTagging;
            }
        }

        public override string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            StringBuilder sb = new StringBuilder();
            int currentChars = currentLineLength;

            WhitespaceTriState newlineSetting = NewlineSetting;

            bool first = currentChars == indent;

            string lastIndentString = "".PadRight(indent);
            if (IndentSetting) indent += IndentSize;
            string indentString = "".PadRight(indent);


            if (newlineSetting == WhitespaceTriState.Never) maxLineLength = int.MaxValue;

            foreach (var child in Contents)
            {
                // Is it a word?
                if (child is WordNode)
                {
                    // Yep

                    // Get the word text so we know how long it will be.
                    string childText = child.ToXml(0, maxLineLength, indent);

                    // Are we the first text?
                    if (first)
                    {
                        // Yep

                        // Clear that
                        first = false;

                        // Append our text (no matter how long)
                        sb.Append(childText);

                        // Set the length
                        currentChars += childText.Length;
                    }
                    else
                    {
                        // Not the first

                        // Will this text overflow the line?
                        if (currentChars + childText.Length + 1 > maxLineLength)
                        {
                            // Yes

                            // If the word is so big it fills the line then write it anyway
                            if (currentChars == indent)
                            {
                                sb.Append(childText);
                                sb.Append("\n" + indentString);
                                currentChars = indent;
                                continue;
                            }

                            // Break the line here
                            sb.Append("\n" + indentString);

                            // And put the text on the next line
                            sb.Append(childText);

                            // And reset the length
                            currentChars = indent + childText.Length;
                        }
                        else
                        {
                            // No

                            // Add a space
                            sb.Append(" ");

                            // Add the text
                            sb.Append(childText);

                            // Add to the length
                            currentChars += 1 + childText.Length;
                        }
                    }
                    continue;
                    
                }

                // Not a word, so should be an element of some kind.

                // If it's one that starts a new line then do that.
                if (child is ExampleNode ||
                    child is ParaNode ||
                    child is ListNode ||
                    child is CodeNode)
                {

                    // If its a paragraph and it's the only child of the parent then take out the tags
                    ParaNode para = child as ParaNode;
                    if (para != null && Contents.Count == 1)
                    {
                        para.NoTags = true;
                        // Also, in no tag mode, we dont put the start/end new line
                    }
                    else
                    {
                        // End the previous line (if there was one).
                        if (first) first = false;
                        else if (!_endsWithNewline.IsMatch(sb.ToString()))
                        {
                            sb.Append("\n" + indentString);
                        }
                    }

                    // Write the sub element
                    sb.Append(child.ToXml(indent, maxLineLength, indent));

                    // And start a new line
                    if (para == null || !para.NoTags)
                    {
                        sb.Append("\n" + indentString);
                        // And reset the count
                        currentChars = indent;
                    }
                              

                    continue;
                }

                // It's an inline block

                // Get the text assuming it will be on this line (include extra 1 for space).
                ExtendedBlockNode extendedChild = child as ExtendedBlockNode;
                string inlineText;
                if (extendedChild != null && extendedChild.NewlineSetting == WhitespaceTriState.Never )
                {
                    // They want to always use a single line so make sure we pass in 0 as the current line length.
                    inlineText = child.ToXml(0, maxLineLength, indent);
                } else
                {
                    inlineText = child.ToXml(currentChars + 1, maxLineLength, indent);
                }

                // Does it contain newlines?
                if (inlineText.Contains("\n"))
                {
                    // Yes

                    // So work out the length of the unterminated line.
                    int lastLineLength = inlineText.Length - inlineText.LastIndexOf('\n');

                    // Append space
                    if (first) first = false;
                    else
                    {
                        sb.Append(" ");
                    }

                    // Append the text.
                    sb.Append(inlineText);

                    // Set the line length
                    currentChars = lastLineLength;
                    continue;
                }

                // Make sure it fits on this line.
                if (currentChars + inlineText.Length + 1 > maxLineLength)
                {
                    // If the word is so big it fills the line then write it anyway
                    if (currentChars == indent)
                    {
                        sb.Append(inlineText);
                        sb.Append("\n" + indentString);
                        currentChars = indent;
                        first = false;
                        continue;
                    }

                    // Break the line here
                    sb.Append("\n" + indentString);

                    // And put the text on the next line
                    sb.Append(inlineText);

                    // And reset the length
                    currentChars = indent + inlineText.Length;
                } else
                {
                    // Append space
                    if (currentChars != indent) 
                    {
                        sb.Append(" ");
                        currentChars += 1;
                    }

                    // No newlines so its a continuation of the current line
                    sb.Append(inlineText);
                    currentChars += inlineText.Length;
                }
                first = false;
            }

            // Get the total contents
            string contents = sb.ToString();

            if (_endsWithNewline.IsMatch(contents))
            {
                contents = contents.TrimEnd();
            }

            // And get the start and end tags
            string startTag = GetStartTag();
            string endTag = GetEndTag();

            // See if it'll all fit on one line
            if (contents.Contains("\n") ||
                startTag.Length + contents.Length + endTag.Length > maxLineLength - currentLineLength ||
                newlineSetting == WhitespaceTriState.Always)
            {
                // doesn't fit on one line so split the tags onto separate lines

                // Make sure the tags actually have a value
                if (startTag == "" && endTag == "")
                {
                    return contents;
                }
                if (contents == "" && endTag == "")
                {
                    return string.Format("{0}", startTag);
                }

                return string.Format("{0}\n{1}{2}\n{3}{4}", startTag, indentString, contents, lastIndentString, endTag);
            }

            if (contents == "" && endTag == "")
            {
                return string.Format("{0}", startTag);
            }
            
            // Should all fit on one line so do that
            return string.Format("{0}{1}{2}", startTag, contents, endTag);

        }

        public override void InsertMissingTags()
        {
            //foreach (var child in Contents) child.InsertMissingTags();

            List<IXmlCommentNode> newContents = new List<IXmlCommentNode>();

            List<IXmlCommentNode> currentLine = new List<IXmlCommentNode>();
            ListNode list = null;
            DescriptionNode desc = null;
            bool lineStart = true;
            foreach (IXmlCommentNode xmlCommentNode in Contents)
            {
                // Is it a new line
                NewLineNode newLine = xmlCommentNode as NewLineNode;
                if (newLine != null)
                {
                    // Yep

                    // Are we at the start of a line?
                    if (lineStart)
                    {
                        // Double new line

                        if (list == null)
                        {
                            // not in a list, so this the end of a paragraph and the start of something else
                            if (currentLine.Count > 0)
                            {
                                ParaNode para = new ParaNode(Options);
                                para.Contents = currentLine;
                                newContents.Add(para);
                                currentLine = new List<IXmlCommentNode>();
                            }
                        }
                        else
                        {
                            // We are in a list

                            // End the last item if there was one
                            if (desc != null && desc.Contents.Count > 0)
                            {
                                ItemNode item = new ItemNode(Options);
                                item.Description = desc;
                                list.Items.Add(item);
                                desc = null;
                            }

                        }
                    }
                    lineStart = true;
                    continue;
                }

                WordNode word = xmlCommentNode as WordNode;
                if (word != null)
                {
                    if (lineStart)
                    {
                        // At the start of a line

                        bool isBullet = _bulletItemRegex.IsMatch(word.Word);
                        bool isNumber = _numberItemRegex.IsMatch(word.Word);

                        // See if it's a list item
                        if (isBullet || isNumber)
                        {

                            // This is a new list item.

                            // Check if we're already in a list and, if not then start a new one.
                            if (list == null)
                            {
                                // Not in a list - write the previous paragraph
                                if (currentLine.Count > 0)
                                {
                                    ParaNode para = new ParaNode(Options);
                                    para.Contents = currentLine;
                                    newContents.Add(para);
                                    currentLine = new List<IXmlCommentNode>();
                                }

                                list = new ListNode(Options);
                                if (isBullet)
                                {
                                    list.Type = ListTypes.Bullet;
                                } else
                                {
                                    list.Type = ListTypes.Number;
                                }
                            }
                            else
                            {
                                // Do we have a previous item?
                                if (desc != null && desc.Contents.Count > 0)
                                {
                                    // yep so close that
                                    ItemNode item = new ItemNode(Options);
                                    item.Description = desc;
                                    list.Items.Add(item);
                                }

                                // Is the new item the same type as the old list?
                                if ((isBullet && list.Type != ListTypes.Bullet) ||
                                    (isNumber && list.Type != ListTypes.Number))
                                {
                                    // Different list type so end the old list and start a new one.
                                    if (list.Items.Count > 0) newContents.Add(list);
                                    list = new ListNode(Options);
                                    if (isBullet)
                                    {
                                        list.Type = ListTypes.Bullet;
                                    }
                                    else
                                    {
                                        list.Type = ListTypes.Number;
                                    }

                                }
                            }

                            // Create the new item
                            desc = new DescriptionNode(Options);
                        }
                        else
                        {
                            // Just a regular word.
                            
                            // See if it's an identifier
                            if (IdentifierResolver.IsIdentifier(Options.Declaration, Options.Solution, word.Word, Options.IdentifierLookupScope ) ||
                                IdentifierResolver.IsKeyword(Options.Declaration, Options.Solution, word.Word))
                            {
                                if (!IdentifiersToIgnore.Any(x => x.IsMatch(word.Word)))
                                {
                                    IList<string> replaceFormats = IdentifierResolver.GetReplaceFormats(
                                        Options.Declaration, Options.Solution, word.Word,
                                        Options.IdentifierLookupScope);

                                    if (replaceFormats.Count == 1)
                                    {
                                        UnknownTagNode seeNode = new UnknownTagNode(Options);
                                        seeNode.StartTag = string.Format(replaceFormats[0], word.Word);
                                        seeNode.EndTag = "";
                                        currentLine.Add(seeNode);
                                        lineStart = false;
                                        continue;
                                    }
                                }

                            }

                            // Are we in a list?
                            if (list == null)
                            {
                                // Nope so just append the word to the current line
                                currentLine.Add(word);
                                lineStart = false;
                                continue;
                            }

                            // yep - in a list

                            // Are we in an item?
                            if (desc == null)
                            {
                                // no - so that's the end of the list
                                if (list.Items.Count > 0) newContents.Add(list);
                                list = null;
                                currentLine.Add(word);
                                lineStart = false;
                                continue;
                            }

                            // Yes in an item so append to that
                            desc.Contents.Add(word);

                        }
                        lineStart = false;
                        continue;
                    }

                    // Middle of a line, just append this word to whatever the current thing is

                    // But check if it's an identifier that needs replacing first.
                    if (IdentifierResolver.IsIdentifier(Options.Declaration, Options.Solution, word.Word, Options.IdentifierLookupScope) ||
                        IdentifierResolver.IsKeyword(Options.Declaration, Options.Solution, word.Word))
                    {
                        if (!IdentifiersToIgnore.Any(x => x.IsMatch(word.Word)))
                        {
                            IList<string> replaceFormats = IdentifierResolver.GetReplaceFormats(
                                Options.Declaration, Options.Solution, word.Word,
                                Options.IdentifierLookupScope);

                            if (replaceFormats.Count == 1)
                            {
                                UnknownTagNode seeNode = new UnknownTagNode(Options);
                                seeNode.StartTag = string.Format(replaceFormats[0], word.Word);
                                seeNode.EndTag = "";
                                currentLine.Add(seeNode);
                                lineStart = false;
                                continue;
                            }
                        }

                    }

                    if (desc == null)
                    {
                        // Just a regular paragraph
                        currentLine.Add(word);
                    }
                    else
                    {
                        // List item
                        desc.Contents.Add(word);
                    }
                    continue;
                }

                // not a word, should then be some other tag
                
                // ask it to insert missing tags
                xmlCommentNode.InsertMissingTags();

                // Now look at what we're currently adding
                if (list == null)
                {
                    // Regular paragraph.

                    // Is it an inline tag?
                    if (xmlCommentNode is ExampleNode ||
                        xmlCommentNode is ParaNode ||
                        xmlCommentNode is ListNode ||
                        xmlCommentNode is CodeNode)
                    {
                        // Not an inline tag so end the current paragraph
                        if (currentLine.Count > 0)
                        {
                            ParaNode para = new ParaNode(Options);
                            para.Contents = currentLine;
                            newContents.Add(para);
                        }

                        // And just add this node directly into the new contentts
                        newContents.Add(xmlCommentNode);

                        // And start a new line
                        currentLine = new List<IXmlCommentNode>();
                        lineStart = false;
                    } 
                    else
                    {
                        // Yep inline tag so just add to the current line
                        currentLine.Add(xmlCommentNode);
                        lineStart = false;
                    }
                }
                else
                {
                    // In a list
                    if (desc == null)
                    {
                        // not in an item so this ends the list
                        // no - so that's the end of the list
                        if (list.Items.Count > 0) newContents.Add(list);
                        list = null;
                        currentLine = new List<IXmlCommentNode>();
                        currentLine.Add(xmlCommentNode);
                        lineStart = false;
                    }
                    else
                    {
                        // In an item so just add to that item
                        desc.Contents.Add(xmlCommentNode);
                        lineStart = false;
                    }
                }
                
            }

            // Commit whatever the last object was
            if (list == null)
            {
                // Just a regular paragraph

                // Make sure there's actually some content
                if (currentLine.Count > 0)
                {
                    ParaNode para = new ParaNode(Options);
                    para.Contents = currentLine;
                    newContents.Add(para);
                }
            }
            else
            {
                // List in progress
   
                // Do we have an item?
                if (desc != null)
                {
                    // yep so close that
                    ItemNode item = new ItemNode(Options);
                    item.Description = desc;
                    list.Items.Add(item);
                }

                // add the list if it has items
                if (list.Items.Count > 0)
                {
                    newContents.Add(list);
                }
            }
            Contents = newContents;
        }


        private void AddTextNode(XmlText text)
        {
            List<string> lines =  new List<string>( text.InnerText.Split('\n'));

            foreach (string line in lines)
            {
                Contents.Add(new NewLineNode());

                string normalisedLine = _whitespaceRegex.Replace(line, " ").Trim();

                string[] words = normalisedLine.Split(' ');

                foreach (string word in words)
                {
                    // Ignore emtpy words
                    if (word == "") continue;

                    WordNode node = new WordNode {Word = word};
                    Contents.Add(node);
                }

            }
            if (lines[lines.Count - 1].Trim(new[] {' ', '\t'}).EndsWith("\n"))
            {
                // Append a last newline
                Contents.Add(new NewLineNode());
            }
        }

        public void AddNode(XmlNode node)
        {
            XmlElement element = node as XmlElement;
            if (element != null)
            {
                IXmlCommentNode childNode;
                switch (element.Name.ToLower())
                {
                    case "p":
                    case "para":
                        childNode = new ParaNode(Options);
                        break;
                    case "ul":
                        {
                            ListNode list = new ListNode(Options);
                            list.Type = ListTypes.Bullet;
                            childNode = list;
                            break;
                        }
                    case "ol":
                        {
                            ListNode list = new ListNode(Options);
                            list.Type = ListTypes.Number;
                            childNode = list;
                            break;
                        }
                    case "list":
                        childNode = new ListNode(Options);
                        break;
                    case "c":
                        childNode = new CNode(Options);
                        break;
                    case "code":
                        childNode = new CodeNode(Options);
                        break;
                    case "see":
                        childNode = new SeeNode(Options);
                        break;
                    case "seealso":
                        childNode = new SeeAlsoNode(Options);
                        break;
                    case "paramref":
                        childNode = new ParamRefNode(Options);
                        break;
                    case "typeparamref":
                        childNode = new TypeParamRefNode(Options);
                        break;
                    case "example":
                        childNode = new ExampleNode(Options);
                        break;
                    default:
                        // If we see another tag then they should have escaped it so do that for them
                        string outerXml = element.OuterXml;
                        string startTag = outerXml.Substring(0, outerXml.IndexOf('>') + 1).Trim();
                        //startTag = startTag.Replace("<", "&lt;").Replace(">", "&gt;");
                        string endTag = element.IsEmpty? "" : string.Format("</{0}>", element.LocalName);
                        childNode = new UnknownTagNode(Options) { StartTag = startTag, EndTag = endTag };
                        break;
                }
                childNode.FromXml(element);
                Contents.Add(childNode);

                return;
            }

            XmlText text = node as XmlText;
            if (text != null)
            {
                AddTextNode(text);
            }
        }

        public override void FromXml(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                AddNode(child);
            }
        }

    }

    public class SummaryNode : ExtendedBlockNode
    {
        public SummaryNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.SummaryTagOnNewLine; }
        }
        public override bool IndentSetting
        {
            get { return Options.Settings.SummaryTagIndent; }
        }
        public override string GetStartTag()
        {
            return "<summary>";
        }

        public override string GetEndTag()
        {
            return "</summary>";
        }
    }

    public class RemarksNode : ExtendedBlockNode
    {
        public RemarksNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.RemarksTagOnNewLine; }
        }
        public override bool IndentSetting
        {
            get { return Options.Settings.RemarksTagIndent; }
        }
        public override string GetStartTag()
        {
            return "<remarks>";
        }

        public override string GetEndTag()
        {
            return "</remarks>";
        }
    }

    public class ValueNode : ExtendedBlockNode
    {
        public ValueNode(XmlCommentOptions options) : base(options) {}

        public override string GetStartTag()
        {
            return "<value>";
        }

        public override string GetEndTag()
        {
            return "</value>";
        }
    }

    public class ExampleNode : ExtendedBlockNode
    {

        public ExampleNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.ExampleTagOnNewLine; }
        }
        public override bool IndentSetting
        {
            get { return Options.Settings.ExampleTagIndent; }
        }
        public override string GetStartTag()
        {
            return "<example>";
        }

        public override string GetEndTag()
        {
            return "</example>";
        }
    }
    public class ReturnsNode : ExtendedBlockNode
    {
        public ReturnsNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.ReturnsTagOnNewLine; }
        }
        public override bool IndentSetting
        {
            get { return Options.Settings.ReturnsTagIndent; }
        }

        public override string GetStartTag()
        {
            return "<returns>";
        }

        public override string GetEndTag()
        {
            return "</returns>";
        }
    }

    public class ParaNode : ExtendedBlockNode
    {
        public ParaNode(XmlCommentOptions options) : base(options) {}

        public bool NoTags = false;

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.ParaTagOnNewLine; }
        }
        public override bool IndentSetting
        {
            get
            {
                if (NoTags) return false;
                return Options.Settings.ParaTagIndent;
            }
        }

        public override string GetStartTag()
        {
            if (NoTags) return "";
            return "<para>";
        }

        public override string GetEndTag()
        {
            if (NoTags) return "";
            return "</para>";
        }
    }

    public abstract class SeeNodeBase : ExtendedBlockNode
    {

        protected SeeNodeBase(XmlCommentOptions options) : base(options) {}

        [XmlAttribute("cref")]
        public string CRef { get; set; }

        public override void FromXml(XmlNode node)
        {
            XmlElement element = node as XmlElement;
            if (element == null) return;

            if (element.HasAttribute("cref", ""))
            {
                CRef = element.GetAttribute("cref", "");
            }

            base.FromXml(element);
        }

        public override void InsertMissingTags()
        {
            // See if we can shorten the CRef at all.
            CRef = IdentifierResolver.ContractCRef(CRef, Options.Declaration, Options.Solution,
                                            Options.IdentifierLookupScope);

            base.InsertMissingTags();
        }

        public abstract string GetSingleTag();

        public override string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            if (Contents.Count == 0)
            {
                return GetSingleTag();
            }
            return base.ToXml(currentLineLength, maxLineLength, indent);
        }
    }

    public class ExceptionNode : SeeNodeBase
    {

        public ExceptionNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.ExceptionTagOnNewLine; }
        }

        public override string GetStartTag()
        {
            return string.Format("<exception cref=\"{0}\">", CRef);
        }

        public override string GetEndTag()
        {
            return "</exception>";
        }

        public override string GetSingleTag()
        {
            return string.Format("<exception cref=\"{0}\" />", CRef);
        }
    }
    public class PermissionNode : SeeNodeBase
    {
        public PermissionNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.PermissionTagOnNewLine; }
        }

        public override string GetStartTag()
        {
            return string.Format("<permission cref=\"{0}\">", CRef);
        }

        public override string GetEndTag()
        {
            return "</permission>";
        }

        public override string GetSingleTag()
        {
            return string.Format("<permission cref=\"{0}\" />", CRef);
        }

    }
    public class SeeNode : SeeNodeBase
    {

        public SeeNode(XmlCommentOptions options) : base(options) {}

        [XmlAttribute("langword")]
        public string LangWord { get; set; }

        public override WhitespaceTriState NewlineSetting
        {
            get { return WhitespaceTriState.Never; }
        }

        public override void FromXml(XmlNode node)
        {
            XmlElement element = node as XmlElement;
            if (element == null) return;

            if (element.HasAttribute("langword", ""))
            {
                LangWord = element.GetAttribute("langword", "");
            }

            base.FromXml(element);
        }

        public override string GetStartTag()
        {
            if (LangWord != null) return string.Format("<see langword=\"{0}\">", LangWord);
            return string.Format("<see cref=\"{0}\">", CRef);
        }

        public override string GetEndTag()
        {
            return "</see>";
        }

        public override string GetSingleTag()
        {
            if (LangWord != null) return string.Format("<see langword=\"{0}\" />", LangWord);
            return string.Format("<see cref=\"{0}\" />", CRef);
        }

    }
    public class SeeAlsoNode : SeeNodeBase
    {

        public SeeAlsoNode(XmlCommentOptions options) : base(options) {}

        public override string GetStartTag()
        {
            return string.Format("<seealso cref=\"{0}\">", CRef);
        }

        public override string GetEndTag()
        {
            return "</seealso>";
        }

        public override string GetSingleTag()
        {
            return string.Format("<seealso cref=\"{0}\" />", CRef);
        }

    }

    public class TermNode : ExtendedBlockNode
    {

        public TermNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.TermTagOnNewLine; }
        }
        public override bool IndentSetting
        {
            get { return Options.Settings.TermTagIndent; }
        }

        public override string GetStartTag()
        {
            return "<term>";
        }

        public override string GetEndTag()
        {
            return "</term>";
        }
    }

    public class DescriptionNode : ExtendedBlockNode
    {

        public DescriptionNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.DescriptionTagOnNewLine; }
        }
        public override bool IndentSetting
        {
            get { return Options.Settings.DescriptionTagIndent; }
        }

        public override string GetStartTag()
        {
            return "<description>";
        }

        public override string GetEndTag()
        {
            return "</description>";
        }
    }

    public abstract class ParamNodeBase : ExtendedBlockNode
    {

        protected ParamNodeBase(XmlCommentOptions options) : base(options) {}
    
        [XmlAttribute("name")]
        public string Name { get; set; }

        public override void FromXml(XmlNode node)
        {
            XmlElement element = node as XmlElement;
            if (element == null) return;

            if (element.HasAttribute("name", ""))
            {
                Name = element.GetAttribute("name", "");
            }

            base.FromXml(element);
        }

        public override IEnumerable<Regex> IdentifiersToIgnore
        {
            get
            {
                List<Regex> result = new List<Regex>() { new Regex("^" + Name + "$")};
                result.AddRange(base.IdentifiersToIgnore);
                return result;
            }
        }
    }

    public class ParamNode : ParamNodeBase
    {

        public ParamNode(XmlCommentOptions options) : base(options) {}


        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.ParamTagOnNewLine; }
        }

        public override string GetStartTag()
        {
            return string.Format("<param name=\"{0}\">", Name);
        }

        public override string GetEndTag()
        {
            return "</param>";
        }


    }
    public class TypeParamNode : ParamNodeBase
    {

        public TypeParamNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting
        {
            get { return Options.Settings.TypeParamTagOnNewLine; }
        }

        public override string GetStartTag()
        {
            return string.Format("<typeparam name=\"{0}\">", Name);
        }

        public override string GetEndTag()
        {
            return "</typeparam>";
        }
    }

    public abstract class ParamRefNodeBase : XmlCommentNodeBase
    {

        protected ParamRefNodeBase(XmlCommentOptions options) : base(options) {}

        [XmlAttribute("name")]
        public string Name { get; set; }

        public override void InsertMissingTags()
        {
        }

        public override void FromXml(XmlNode node)
        {
            XmlElement element = node as XmlElement;
            if (element == null) return;

            if (element.HasAttribute("name", ""))
            {
                Name = element.GetAttribute("name", "");
            }
        }
    }

    public class ParamRefNode : ParamRefNodeBase
    {
        public ParamRefNode(XmlCommentOptions options) : base(options) {}

        public override string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            return string.Format("<paramref name=\"{0}\" />", Name);
        }
    }
    public class TypeParamRefNode : ParamRefNodeBase
    {

        public TypeParamRefNode(XmlCommentOptions options) : base(options) {}

        public override string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            return string.Format("<typeparamref name=\"{0}\" />", Name);
        }
    }


    public enum ListTypes
    {
        Bullet,
        Number,
        Table,
        None // Note this is the default - should never be used.
    }

    public class ListNode : XmlCommentNodeBase
    {
        public ListTypes Type { get; set; }

        public ListHeaderNode ListHeader { get; set; }

        public List<ItemNode> Items { get; set; }

        public ListNode(XmlCommentOptions options) : base (options)
        {
            Type = ListTypes.None;
            Items = new List<ItemNode>();
        }

        public WhitespaceTriState NewlineSetting { get { return Options.Settings.ListTagOnNewLine; } }
        public bool IndentSetting { get { return Options.Settings.ListTagIndent; } }
        public int IndentSize { get { return 4; } }

        public override void InsertMissingTags()
        {
            if (ListHeader != null) ListHeader.InsertMissingTags();
            foreach (var item in Items) item.InsertMissingTags();
        }

        public override void FromXml(XmlNode node)
        {
            XmlElement element = node as XmlElement;
            if (element == null) return;

            if (Type == ListTypes.None)
            {
                Type = ListTypes.Bullet;
                if (element.HasAttribute("type", ""))
                {
                    string typeString = element.GetAttribute("type", "");
                    switch (typeString.ToLower())
                    {
                        case "ordered":
                        case "numeric":
                        case "number":
                            Type = ListTypes.Number;
                            break;
                        case "table":
                            Type = ListTypes.Table;
                            break;
                    }
                }
            }

            foreach (XmlNode child in element.ChildNodes)
            {
                XmlElement childElement = child as XmlElement;
                if (childElement != null)
                {

                    switch (childElement.LocalName.ToLower())
                    {
                        case "listheader":
                            if (ListHeader != null) continue; // Ignore if we already have a header
                            ListHeaderNode listHeader = new ListHeaderNode(Options);
                            listHeader.FromXml(childElement);
                            ListHeader = listHeader;
                            break;
                        case "li":
                        case "item":
                            {
                                ItemNode item = new ItemNode(Options);
                                item.FromXml(childElement);
                                Items.Add(item);
                                break;
                            }
                        default:
                            {
                                // Just stick other tags inside an item
                                ItemNode item = new ItemNode(Options);
                                item.Description = new DescriptionNode(Options);
                                item.Description.AddNode(childElement);
                                Items.Add(item);
                            }
                            break;

                    }
                    continue;
                }

                // Everything else is bogus but add it as an item
                ItemNode fakeItem = new ItemNode(Options);
                fakeItem.Description = new DescriptionNode(Options);
                fakeItem.Description.AddNode(child);
                Items.Add(fakeItem);
            }
        }

        public override string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            StringBuilder sb = new StringBuilder();

            string type;
            switch (Type)
            {
                case ListTypes.Number:
                    type = "number";
                    break;
                case ListTypes.Table:
                    type = "table";
                    break;
                default:
                    type = "bullet";
                    break;
            }
            string lastIndentString = "".PadRight(indent);
            if (IndentSetting) indent += IndentSize;
            string indentString = "".PadRight(indent);
            sb.Append(string.Format("<list type=\"{0}\">\n{1}", type, indentString));
            if (ListHeader != null)
            {
                sb.Append(ListHeader.ToXml(indent, maxLineLength, indent));
                sb.Append("\n" + indentString);
            }
            for (int i = 0; i < Items.Count; i++)
            {
                ItemNode itemNode = Items[i];
                sb.Append(itemNode.ToXml(indent, maxLineLength, indent));
                sb.Append("\n");
                if (i == Items.Count - 1) sb.Append(lastIndentString);
                else sb.Append(indentString);
            }
            sb.Append("</list>");
            return sb.ToString();
        }
    }

    public abstract class ItemNodeBase : XmlCommentNodeBase
    {
        protected ItemNodeBase(XmlCommentOptions options) : base(options) {}

        public TermNode Term { get; set; }

        public DescriptionNode Description { get; set; }

        public virtual WhitespaceTriState NewlineSetting { get { return WhitespaceTriState.Always; } }
        public virtual bool IndentSetting { get { return true; } }
        public virtual int IndentSize { get { return 4; } }

        public override void InsertMissingTags()
        {
            if (Term != null) Term.InsertMissingTags();
            if (Description != null) Description.InsertMissingTags();
        }

        public override void FromXml(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                XmlElement childElement = child as XmlElement;
                if (childElement != null)
                {
                    switch (childElement.LocalName.ToLower())
                    {
                        case "term":
                            if (Term != null) continue; // Ignore if we already have a header
                            TermNode term = new TermNode(Options);
                            term.FromXml(childElement);
                            Term = term;
                            break;
                        case "description":
                            if (Description != null) continue; // Ignore if we already have a header
                            DescriptionNode desc = new DescriptionNode(Options);
                            desc.FromXml(childElement);
                            Description = desc;
                            break;
                    }
                    continue;
                }

                XmlText text = node as XmlText;
                if (text != null)
                {
                    if (Description == null) Description = new DescriptionNode(Options);
                    Description.AddNode(node);
                }
            }
        }

        public abstract string GetStartTag();
        public abstract string GetEndTag();

        public override string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            // Get the various text strings

            // Start tag
            string startTag = GetStartTag();

            // End tag
            string endTag = GetEndTag();

            string lastIndentString = "".PadRight(indent);
            if (IndentSetting) indent += IndentSize;
            string indentString = "".PadRight(indent);

            // The term section
            string term = "";
            if (Term != null) term = Term.ToXml(indent, maxLineLength, indent);

            // The description section
            string desc = "";
            if (Description != null) desc = Description.ToXml(indent, maxLineLength, indent);


            // If any of the sections used a new line OR the whole thing is too long to fit on one line then...
            if (term.Contains("\n") ||
                desc.Contains("\n") ||
                currentLineLength + startTag.Length + term.Length + desc.Length + endTag.Length > maxLineLength ||
                NewlineSetting == WhitespaceTriState.Always )
            {
                StringBuilder sb1 = new StringBuilder();

                sb1.Append(startTag);
                if (term.Length != 0)
                {
                    sb1.Append("\n");
                    sb1.Append(indentString);
                    sb1.Append(term);
                }
                if (desc.Length != 0)
                {
                    sb1.Append("\n");
                    sb1.Append(indentString);
                    sb1.Append(desc);
                }
                sb1.Append("\n");
                sb1.Append(lastIndentString);
                sb1.Append(endTag);

                return sb1.ToString();
            }

            // Looks like it should all fit.
            return string.Format("{0}{1}{2}{3}", startTag, term, desc, endTag);
        }
    }

    public class ListHeaderNode : ItemNodeBase
    {

        public ListHeaderNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting { get { return Options.Settings.ListHeaderTagOnNewLine; } }
        public override bool IndentSetting { get { return Options.Settings.ListHeaderTagIndent; } }
        public override int IndentSize { get { return 4; } }

        public override string GetStartTag()
        {
            return "<listheader>";
        }

        public override string GetEndTag()
        {
            return "</listheader>";
        }
    }
    public class ItemNode : ItemNodeBase
    {

        public ItemNode(XmlCommentOptions options) : base(options) {}

        public override WhitespaceTriState NewlineSetting { get { return Options.Settings.ItemTagOnNewLine; } }
        public override bool IndentSetting { get { return Options.Settings.ItemTagIndent; } }
        public override int IndentSize { get { return 4; } }

        public override string GetStartTag()
        {
            return "<item>";
        }

        public override string GetEndTag()
        {
            return "</item>";
        }
    }

    public abstract class CodeNodeBase : XmlCommentNodeBase
    {
        private static readonly Regex _newlineSpace = new Regex("\n ");
        private static readonly Regex _newlineNewline = new Regex("\n\n");

        public string Lang { get; set; }

        /// <summary>
        /// Sandcastle help file builder attribute
        /// 
        /// This attribute allows you to add a title that appears before the code block. An example of its use would be to label the example with a description. If omitted and the defaultTitle attribute on the code block component's colorizer element is true, the language name will appear for the title. If it is set to false, no title will appear. If using default titles and you do not want a title on a particular block, set the title attribute to a single space (" ").
        /// </summary>
        public string Title { get; set; }

        public string Text { get; set; }

        public string Source { get; set; }

        public string Region { get; set; }

        public bool? RemoveRegionMarkers { get; set; }

        /// <summary>
        /// Sandcastle help file builder attribute
        /// 
        /// This attribute allows you to override the default setting in the component's configuration. For example, if the default setting is false to turn off line numbering, you can add numberLines="true" to enable numbering on a specific code example.
        /// </summary>
        public bool? NumberLines { get; set; }

        /// <summary>
        /// Sandcastle help file builder attribute
        /// 
        /// This attribute allows you to override the default setting in the component's configuration. For example, if the default setting is false to not add collapsible regions, you can add outlining="true" to enable collapsible regions on a specific code example. Note that if a code block contains no #region or #if blocks, outlining is automatically disabled and it will not reserve space in the margin for the markers.
        /// </summary>
        public bool? Outlining { get; set; }

        /// <summary>
        /// Sandcastle help file builder attribute
        /// 
        /// When set to true, this attribute allows you to tell the code colorizer to preserve <see> tags within the code so that they can be rendered as clickable links to the related topic. If set to false, the default, any <see> tags within the code will be colorized and passed through as literal text. When using this option, you may find that you need to specify inner text for the <see> tag so that the link text appears as you want it. If the self-closing version of the tag is used, Sandcastle will generally set the link text to the name of the item plus any parameters if it is a generic type or takes parameters which may not be appropriate within a code sample.
        /// </summary>
        public bool? KeepSeeTags { get; set; }

        /// <summary>
        /// Sandcastle help file builder attribute
        /// 
        /// When the code blocks are formatted, tab characters are replaced with a set number of spaces to preserve formatting. This attribute can be used to override the default setting for a language which is specified in the syntax file. For example, if the default tab size for a language is four, adding tabSize="8" will force it to use eight spaces instead. If set to zero, the syntax file setting is used. This attribute sets the default tab size for unknown languages when used in the component's configuration.
        /// </summary>
        public int? TabSize { get; set; }

        /// <summary>
        /// Sandcastle help file builder attribute
        /// 
        /// This attribute allows you to specify whether or not the code block should be connected to the language filter (VS2005 only). If omitted or set to true, the code block will be connected to the appropriate language filter if it is present. If set to false, it is not connected and will remain visible at all times.
        /// </summary>
        public bool? Filter { get; set; }

        private static readonly List<string> ValidLanguages = new List<string>()
                                                                  {
                                                                      "all",
                                                                      "cs",
                                                                      "c#",
                                                                      "csharp",
                                                                      "cpp",
                                                                      "cpp#",
                                                                      "c++",
                                                                      "cplusplus",
                                                                      "c",
                                                                      "fs",
                                                                      "f#",
                                                                      "fsharp",
                                                                      "fscript",
                                                                      "ecmascript",
                                                                      "js",
                                                                      "javascript",
                                                                      "jscript",
                                                                      "jscript#",
                                                                      "jscriptnet",
                                                                      "jscript.net",
                                                                      "vb",
                                                                      "vb#",
                                                                      "vbnet",
                                                                      "vb.net",
                                                                      "vbs",
                                                                      "vbscript",
                                                                      "htm",
                                                                      "html",
                                                                      "xml",
                                                                      "xsl",
                                                                      "xaml",
                                                                      "jsharp",
                                                                      "j#",
                                                                      "sql",
                                                                      "sql server",
                                                                      "sqlserver",
                                                                      "pshell",
                                                                      "powershell",
                                                                      "ps1",
                                                                      "py",
                                                                      "python"
                                                                  };

        protected CodeNodeBase(XmlCommentOptions options) : base(options)
        {
            Text = "";
        }

        public override void InsertMissingTags()
        {
        }

        public override void FromXml(XmlNode node)
        {
            XmlElement element = node as XmlElement;
            if (element == null) return;

            Lang = null;
            if (element.HasAttribute("lang", ""))
            {
                Lang = element.GetAttribute("lang", "");
            }
            if (element.HasAttribute("language", ""))
            {
                Lang = element.GetAttribute("language", "");
            }
            if (Lang != null && !ValidLanguages.Contains(Lang.ToLower()))
            {
                Lang = null;
            }


            Title = null;
            if (element.HasAttribute("title", ""))
            {
                Title = element.GetAttribute("title", "");
            }

            Source = null;
            if (element.HasAttribute("source", ""))
            {
                Source = element.GetAttribute("source", "");
            }

            Region = null;
            if (element.HasAttribute("region", ""))
            {
                Region = element.GetAttribute("region", "");
            }

            RemoveRegionMarkers = null;
            if (element.HasAttribute("removeRegionMarkers", ""))
            {
                string txt = element.GetAttribute("removeRegionMarkers", "");
                bool result;
                if (bool.TryParse(txt, out result)) RemoveRegionMarkers = result;
            }
            
            NumberLines = null;
            if (element.HasAttribute("numberLines", ""))
            {
                string txt = element.GetAttribute("numberLines", "");
                bool result;
                if (bool.TryParse(txt, out result)) NumberLines = result;
            }

            Outlining = null;
            if (element.HasAttribute("outlining", ""))
            {
                string txt = element.GetAttribute("outlining", "");
                bool result;
                if (bool.TryParse(txt, out result)) Outlining = result;
            }

            KeepSeeTags = null;
            if (element.HasAttribute("keepSeeTags", ""))
            {
                string txt = element.GetAttribute("keepSeeTags", "");
                bool result;
                if (bool.TryParse(txt, out result)) KeepSeeTags = result;
            }

            TabSize = null;
            if (element.HasAttribute("tabSize", ""))
            {
                string txt = element.GetAttribute("tabSize", "");
                int result;
                if (int.TryParse(txt, out result)) TabSize = result;
            }

            Filter = null;
            if (element.HasAttribute("filter", ""))
            {
                string txt = element.GetAttribute("filter", "");
                bool result;
                if (bool.TryParse(txt, out result)) Filter = result;
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                XmlElement childElement = child as XmlElement;
                if (childElement != null)
                {
                    // If we see another tag then they should have escaped it so do that for them
                    string outerXml = childElement.OuterXml;
                    string startTag = outerXml.Substring(0, outerXml.IndexOf('>') - 1).Trim();
                    startTag = startTag.Replace("<", "&lt;").Replace(">", "&gt;");
                    string endTag = childElement.IsEmpty? "" : string.Format("&lt;/{0}&gt;", childElement.LocalName);
                    string innerXml = childElement.InnerXml.Replace("<", "&lt;").Replace(">", "&gt;");
                    Text += startTag + innerXml + endTag;
                    continue;
                }

                XmlText text = child as XmlText;
                if (text != null)
                {
                    if (Text.Length != 0) Text += " ";

                    string txt = text.InnerText;
                    txt = txt.Replace("<", "&lt;").Replace(">", "&gt;");
                    if (txt.Count(x => x == '\n') == _newlineSpace.Matches(txt).Count + _newlineNewline.Matches(txt).Count)
                    {
                        // all new lines start with a space so assume it must have been the indent for the whole comment and strip it.
                        txt = _newlineSpace.Replace(txt, "\n");
                    }

                    // Stupid xml reader doesn't strip the starting space off each line so do that.

                    Text += txt;
                }
            }
        }

        public abstract string GetTagName();

        public virtual string GetStartTag()
        {
            string tag = GetTagName();

            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            sb.Append(tag);

            if (Lang != null)
            {
                sb.Append(string.Format(" lang=\"{0}\"", Lang));
            }
            if (Title != null)
            {
                sb.Append(string.Format(" title=\"{0}\"", Title));
            }

            if (Source != null)
            {
                sb.Append(string.Format(" source=\"{0}\"", Source));
            }

            if (Region != null)
            {
                sb.Append(string.Format(" region=\"{0}\"", Region));
            }

            if (RemoveRegionMarkers != null)
            {
                sb.Append(string.Format(" removeRegionMarkers=\"{0}\"", RemoveRegionMarkers));
            }

            if (NumberLines != null)
            {
                sb.Append(string.Format(" numberLines=\"{0}\"", NumberLines));
            }

            if (Outlining != null)
            {
                sb.Append(string.Format(" outlining=\"{0}\"", Outlining));
            }

            if (KeepSeeTags != null)
            {
                sb.Append(string.Format(" keepSeeTags=\"{0}\"", KeepSeeTags));
            }

            if (TabSize != null)
            {
                sb.Append(string.Format(" tabSize=\"{0}\"", TabSize));
            }

            if (Filter != null)
            {
                sb.Append(string.Format(" filter=\"{0}\"", Filter));
            }

            if (Text.Length == 0) sb.Append(" />");
            else sb.Append(">");
            return sb.ToString();
        }

        public virtual string GetEndTag()
        {
            if (Text.Length == 0) return "";
            string tag = GetTagName();
            return string.Format("</{0}>", tag);
        }


        public override string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetStartTag());
            sb.Append(Text);
            sb.Append(GetEndTag());
            string result = sb.ToString();

            if (result.Contains("\n"))
            {
                // There are new lines.

                // Get the length of the first line
                int firstLineLength = result.IndexOf("\n");
                
                if (currentLineLength + firstLineLength > maxLineLength)
                {
                    return "\n" + result;
                }

                return result;
            }

            // No new lines
            if (currentLineLength + result.Length > maxLineLength)
            {
                return "\n" + result;
            }
            return result;
        }
    }

    public class CodeNode : CodeNodeBase
    {

        public CodeNode(XmlCommentOptions options) : base(options) {}

        public override string GetTagName()
        {
            return "code";
        }
    }

    public class CNode : CodeNodeBase
    {

        public CNode(XmlCommentOptions options) : base(options) {}

        public override string GetTagName()
        {
            return "c";
        }
    }

    public class UnknownTagNode : ExtendedBlockNode
    {

        public UnknownTagNode(XmlCommentOptions options) : base(options) {}

        public string StartTag { get; set; }
        public string EndTag { get; set; }

        public override string GetStartTag()
        {
            return StartTag;
        }

        public override string GetEndTag()
        {
            return EndTag;
        }
    }

    public class WordNode : IXmlCommentNode
    {
        public string Word { get; set; }

        public void InsertMissingTags()
        {
        }

        public string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            return Word;
        }

        public void FromXml(XmlNode node)
        {
        }

        public override string ToString()
        {
            return Word;
        }
    }

    public class NewLineNode : IXmlCommentNode
    {
        public void InsertMissingTags()
        {
        }

        public string ToXml(int currentLineLength, int maxLineLength, int indent)
        {
            return "\n";
        }

        public void FromXml(XmlNode node)
        {
        }
    }
}
