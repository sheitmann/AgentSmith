using System;
using System.Collections.Generic;

namespace AgentSmith.Comments.Reflow
{
    public class ParagraphLine
    {
        private readonly List<ParagraphLineItem> _items = new List<ParagraphLineItem>();

        public void AddItem(ParagraphLineItem item)
        {
            _items.Add(item);
        }

        public IList<ParagraphLineItem> Items
        {
            get
            {
                return _items;
            }
        }

        public ParagraphLineItem LastItem
        {
            get
            {
                if (_items.Count == 0) return null;
                return _items[_items.Count - 1];
            }
        }

        public ParagraphLine TrimStart()
        {
            ParagraphLine newLine = new ParagraphLine();
            int i = 0;
            
            while (i < Items.Count && Items[i].ItemType == ItemType.XmlSpace)
                i++;

            for (; i < Items.Count; i++)
            {
                newLine.AddItem(Items[i]);                
            }

            return newLine;
        }

        public ParagraphLine TrimEnd()
        {
            ParagraphLine newLine = new ParagraphLine();
            int i = Items.Count - 1;

            while (i >=0 && Items[i].ItemType == ItemType.XmlSpace)
                i--;
            
            for (int j=0; j<=i; j++)
            {
                newLine.AddItem(Items[j]);                
            }

            return newLine;
        }

        public ParagraphLine Trim()
        {
            return TrimStart().TrimEnd();
        }
    }
}
