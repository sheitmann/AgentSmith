using System;
using System.Collections.Generic;

namespace AgentSmith.Comments.Reflow
{
    public class Paragraph
    {
        private readonly List<ParagraphLine> _lines = new List<ParagraphLine>();

        public string Offset
        {
            get
            {
                if (Lines.Count == 0 || Lines[0].Items.Count == 0 )
                    return String.Empty;
                ParagraphLineItem firstItem = Lines[0].Items[0];
                int i = 0;
                while (i<firstItem.Text.Length && (firstItem.Text[i] == ' ' || firstItem.Text[i] == '\t'))
                    i++;
                return firstItem.Text.Substring(0, i);                    
            }
        }

        public IList<ParagraphLine> Lines
        {
            get
            {
                return _lines;
            }
        }

        public void Add(ParagraphLine line)
        {
            _lines.Add(line);
        }

        public void AddTagStartLine(string tagName)
        {
            ParagraphLine resultLine = new ParagraphLine();
            resultLine.AddItem(
                new ParagraphLineItem
                {
                    Text = string.Format("<{0}>", tagName),
                    ItemType = ItemType.XmlElement
                });
            Add(resultLine);
        }

        public void AddCustomTagStartLine(string text)
        {
            ParagraphLine resultLine = new ParagraphLine();
            resultLine.AddItem(
                new ParagraphLineItem
                {
                    Text = text,
                    ItemType = ItemType.XmlElement
                });
            Add(resultLine);
        }

        public void AddTagEndLine(string tagName)
        {
            ParagraphLine resultLine = new ParagraphLine();
            resultLine.AddItem(
                new ParagraphLineItem
                {
                    Text = string.Format("</{0}>", tagName),
                    ItemType = ItemType.XmlElement
                });
            Add(resultLine);
        }
    }
}
