using System;
using System.Collections.Generic;

namespace AgentSmith.SpellCheck.NetSpell
{
    internal class MultilingualSpellchecker : ISpellChecker
    {
        private readonly ISpellChecker[] _spellCheckers;
        private ISpellChecker _lastSuccessfulSpellChecker;

        internal MultilingualSpellchecker(ISpellChecker[] spellCheckers)
        {
            _spellCheckers = spellCheckers;
        }

        #region ISpellChecker Members

        public CustomDictionary[] CustomDictionaries
        {
            get
            {
                List<CustomDictionary> customDictionaries = new List<CustomDictionary>();
                foreach (ISpellChecker spellChecker in _spellCheckers)
                {
                    customDictionaries.AddRange(spellChecker.CustomDictionaries);
                }
                return customDictionaries.ToArray();
            }
        }

        public IList<string> Suggest(string word, uint maxSuggestions)
        {
            List<string> suggestions = new List<string>();
            foreach (ISpellChecker spellChecker in _spellCheckers)
            {
                suggestions.AddRange(spellChecker.Suggest(word, maxSuggestions));
                if (suggestions.Count > maxSuggestions)
                {
                    suggestions.RemoveRange((int)maxSuggestions, (int)(suggestions.Count - maxSuggestions));
                    break;
                }
            }
            return suggestions;
        }

        public bool TestWord(string word, bool matchCase)
        {
            if (_lastSuccessfulSpellChecker != null)
            {
                if (_lastSuccessfulSpellChecker.TestWord(word, matchCase))
                {
                    return true;
                }
            }
            
            foreach (ISpellChecker spellChecker in _spellCheckers)
            {
                if (spellChecker != _lastSuccessfulSpellChecker &&
                    spellChecker.TestWord(word, matchCase))
                {
                    _lastSuccessfulSpellChecker = spellChecker;
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}