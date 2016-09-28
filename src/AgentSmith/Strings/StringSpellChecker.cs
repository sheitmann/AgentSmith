using System.Collections.Generic;
using AgentSmith.Comments;
using AgentSmith.Options;
using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentSmith.Strings
{
    internal static class StringSpellChecker
    {
        private static string unescape(string text)
        {
            if (!text.StartsWith("@"))
            {
                //TODO: optimize this
                return text.
                    Replace("\\\\", "  ").
                    Replace("\\a", "  ").
                    Replace("\\b", "  ").
                    Replace("\\f", "  ").
                    Replace("\\n", "  ").
                    Replace("\\r", "  ").
                    Replace("\\t", "  ").
                    Replace("\\v", "  ");
            }
            return text;
        }

        public static void SpellCheck(IDocument document, ITokenNode token, ISpellChecker spellChecker,
                                                       ISolution solution, DefaultHighlightingConsumer consumer, IContextBoundSettingsStore settingsStore, StringSettings settings)
        {
            if (spellChecker == null) return;

            string buffer = unescape(token.GetText());
            ILexer wordLexer = new WordLexer(buffer);
            wordLexer.Start();
            while (wordLexer.TokenType != null)
            {
                string tokenText = wordLexer.GetCurrTokenText();
                if (SpellCheckUtil.ShouldSpellCheck(tokenText, settings.CompiledWordsToIgnore) &&
                    !spellChecker.TestWord(tokenText, true))
                {
                    IClassMemberDeclaration containingElement =
                        token.GetContainingNode<IClassMemberDeclaration>(false);
                    if (containingElement == null ||
                        !IdentifierResolver.IsIdentifier(containingElement, solution, tokenText))
                    {
                        CamelHumpLexer camelHumpLexer = new CamelHumpLexer(buffer, wordLexer.TokenStart, wordLexer.TokenEnd);
                        foreach (LexerToken humpToken in camelHumpLexer)
                        {
                            if (SpellCheckUtil.ShouldSpellCheck(humpToken.Value, settings.CompiledWordsToIgnore) &&
                                !spellChecker.TestWord(humpToken.Value, true))
                            {
								//int start = token.GetTreeStartOffset().Offset + wordLexer.TokenStart;
								//int end = start + tokenText.Length;

								//TextRange range = new TextRange(start, end);
								//DocumentRange documentRange = new DocumentRange(document, range);

								DocumentRange documentRange =
								   token.GetContainingFile().TranslateRangeForHighlighting(token.GetTreeTextRange());
								documentRange = documentRange.ExtendLeft(-wordLexer.TokenStart);
								documentRange = documentRange.ExtendRight(-1 * (documentRange.GetText().Length - tokenText.Length));

                                TextRange textRange = new TextRange(humpToken.Start - wordLexer.TokenStart,
                                    humpToken.End - wordLexer.TokenStart);

								//string word = document.GetText(range);
	                            string word = documentRange.GetText();
	                            consumer.AddHighlighting(
		                            new StringSpellCheckHighlighting(
			                            word,
			                            documentRange,
			                            humpToken.Value,
			                            textRange,
			                            solution,
			                            spellChecker,
			                            settingsStore),
		                            documentRange);
                                break;
                            }
                        }
                    }
                }
                wordLexer.Advance();
            }
        }
    }
}