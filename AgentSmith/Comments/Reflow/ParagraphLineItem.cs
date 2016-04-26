using System;
using System.Text.RegularExpressions;

namespace AgentSmith.Comments.Reflow
{
    public enum ItemType
    {
        XmlSpace,
        XmlElement,
        NonReflowableBlock,
        Text
    }

    public class ParagraphLineItem
    {
        public string Text;
        public ItemType ItemType;

        public string Tag
        {
            get
            {
                if (ItemType != ItemType.XmlElement) return null;

                Regex re = new Regex(@"^\s*<[/]?(\w+).*>\s*$");

                Match match = re.Match(Text);

                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                return null;
            }
        }

        public bool IsEndTag
        {
            get
            {
                if (ItemType != ItemType.XmlElement) return false;
                Regex re = new Regex(@"^\s*</(\w+).*>\s*$");

                Match match = re.Match(Text);
                if (match.Success)
                {
                    return true;
                }
                return false;
            }
        }

        public string FirstLine
        {
            get
            {
                int n = Text.IndexOf("\n", StringComparison.Ordinal);
                return n < 0? Text : Text.Substring(0, n);
            }
        }

        /// <summary>
        /// Determines if given <c>ParagraphLineItem</c> should be forcing 
        /// a new line.
        /// </summary>
        /// <param name="lineItem">
        /// The line item to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if given <c>ParagraphLineItem</c> should be forcing 
        ///  a new line; <c>false</c> otherwise.
        /// </returns>
        public bool IsForcingNewLine
        {
            get
            {
                return (ItemType == ItemType.XmlElement &&
                  Text.Equals("<br/>", StringComparison.CurrentCultureIgnoreCase));
            }
        }
    }
}
