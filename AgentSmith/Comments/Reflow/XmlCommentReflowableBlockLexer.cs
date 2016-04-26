using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith.Comments.Reflow
{
    public class XmlCommentReflowableBlockLexer : IEnumerable<string>
    {
        private readonly XmlDocLexer _docLexer;

        public XmlCommentReflowableBlockLexer(IDocCommentBlock docCommentBlock)
        {
            _docLexer = new XmlDocLexer(docCommentBlock);
            _docLexer.Start();
        }

        public IEnumerator<string> GetEnumerator()
        {
            _docLexer.Start();
            int inCode = 0;
            bool currentTagIsCode = false;
            StringBuilder blockBuilder = new StringBuilder();
            while (_docLexer.TokenType != null)
            {
                if (_docLexer.TokenType == _docLexer.XmlTokenType.TAG_START)
                {
                    if (blockBuilder.Length > 0 && inCode == 0)
                    {
                        yield return blockBuilder.ToString();
                        blockBuilder.Remove(0, blockBuilder.Length);
                    }

                    blockBuilder.Append(_docLexer.TokenText);                    
                    _docLexer.Advance();

                    currentTagIsCode = false;
                    if (_docLexer.TokenType == _docLexer.XmlTokenType.IDENTIFIER &&
                        (_docLexer.TokenText == "code" ||
                         _docLexer.TokenText == "c" ||
                         _docLexer.TokenText == "see" ||
                         _docLexer.TokenText == "typeparamref" ||
                         _docLexer.TokenText == "paramref"))
                    {
                        inCode++;
                        currentTagIsCode = true;
                    }
                    
                    blockBuilder.Append(_docLexer.TokenText);
                    _docLexer.Advance();
                                        
                }
                else if (_docLexer.TokenType == _docLexer.XmlTokenType.TAG_START1)
                {
                    if (blockBuilder.Length > 0 && inCode == 0)
                    {
                        yield return blockBuilder.ToString();
                        blockBuilder.Remove(0, blockBuilder.Length);
                    }

                    blockBuilder.Append(_docLexer.TokenText);
                    _docLexer.Advance();
                    if (_docLexer.TokenType == _docLexer.XmlTokenType.IDENTIFIER &&
                        (_docLexer.TokenText == "code" ||
                         _docLexer.TokenText == "c" ||
                         _docLexer.TokenText == "see" ||
                         _docLexer.TokenText == "typeparamref" ||
                         _docLexer.TokenText == "paramref"))
                    {
                        inCode--;
                    }
                }
                else if (_docLexer.TokenType == _docLexer.XmlTokenType.TAG_END ||
                         _docLexer.TokenType == _docLexer.XmlTokenType.TAG_END1)
                {
                    if (_docLexer.TokenType == _docLexer.XmlTokenType.TAG_END1 && currentTagIsCode)
                    {
                        inCode--;
                    }
                    
                    blockBuilder.Append(_docLexer.TokenText);
                    if (inCode == 0)
                    {
                        yield return blockBuilder.ToString();
                        blockBuilder.Remove(0, blockBuilder.Length);
                    }
                    _docLexer.Advance();
                }
                else
                {
                    blockBuilder.Append(_docLexer.TokenText);
                    _docLexer.Advance();
                }
                
            }

            yield return blockBuilder.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}