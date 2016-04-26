using System;
using System.Collections.Generic;

namespace AgentSmith.SpellCheck.NetSpell
{
    /// <summary>
    /// Spell checker.
    /// </summary>
    public interface ISpellChecker
    {
        CustomDictionary[] CustomDictionaries { get; }

        /// <summary>
        /// Generates suggestions for a misspelled word.
        /// </summary>
        /// <param name="word" type="string">
        /// The word to generate suggestions for.        
        /// </param>
        /// <param name="maxSuggestions">Maximum number of suggestions to produce.</param>        
        IList<string> Suggest(string word, uint maxSuggestions);

        /// <summary>
        /// Checks if the word is in the dictionary.
        /// </summary>
        /// <param name="word" type="string">        
        /// The word to check.        
        /// </param>
        /// <returns>
        /// Returns true if word is spelled correctly.
        /// </returns>
        bool TestWord(string word, bool matchCase);
    }
}