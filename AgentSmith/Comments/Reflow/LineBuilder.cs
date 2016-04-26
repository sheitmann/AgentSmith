using System;
using System.Text;

namespace AgentSmith.Comments.Reflow
{
    public class LineBuilder
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private string _currentLine = "";
        
        public string CurrentLine
        {
            get
            {
                return _currentLine;
            }
        }

        public void Append(string s)
        {                        
            int n = s.LastIndexOf("\n");
            if (n >= 0)
            {
                _sb.Append(_currentLine);
                _sb.Append(s.Substring(0, n + 1));
                _currentLine = s.Substring(n + 1);
            }
            else
            {
                _currentLine += s;
            }
        }

        public void AppendMultilineBlock(string block)
        {              
            Append(block);
        }

        public override string ToString()
        {
            return _sb + _currentLine;
        }
    }
}