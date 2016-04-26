using System.Collections;
using System.Collections.Generic;

namespace AgentSmith.SpellCheck
{
    public class CamelHumpLexer : IEnumerable<LexerToken>
    {
        private readonly string _buffer;
        private readonly int _start;
        private readonly int _end;

        public CamelHumpLexer(string buffer, int start, int end)
        {
            _buffer = buffer;
            _start = start;
            _end = end;
        }

        #region IEnumerable<LexerToken> Members

        IEnumerator<LexerToken> IEnumerable<LexerToken>.GetEnumerator()
        {
            LexerToken currentToken = new LexerToken(_buffer, _start, _start);

            while (currentToken.End < _end)
            {
                char c = _buffer[currentToken.End];
                if ((c == '_' || c == '@' || c == '.'))
                {
                    if (currentToken.Length > 0)
                    {
                        yield return currentToken;
                    }
                    currentToken = new LexerToken(_buffer, currentToken.End + 1, currentToken.End + 1);
                }
                else if (char.IsUpper(c))
                {
                    if (currentToken.Length > 0 && (char.IsLower(_buffer[currentToken.End - 1]) ||
                                                    currentToken.End + 1 < _end &&
                                                    char.IsLower(_buffer[currentToken.End + 1])))
                    {
                        yield return currentToken;
                        currentToken = new LexerToken(_buffer, currentToken.End, currentToken.End);
                    }
                    else
                    {
                        currentToken.End++;
                    }
                }
                else
                {
                    currentToken.End ++;
                }
            }
            if (currentToken.Length > 0)
            {
                yield return currentToken;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<LexerToken>) this).GetEnumerator();
        }

        #endregion        
    }
}