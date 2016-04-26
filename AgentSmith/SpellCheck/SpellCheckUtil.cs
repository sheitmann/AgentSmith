using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AgentSmith.SpellCheck
{
    public static class SpellCheckUtil
    {
        public static bool ShouldSpellCheck(string word, List<Regex> ignoreRegexes = null)
        {

            if (ignoreRegexes != null)
            {
                foreach (Regex re in ignoreRegexes)
                {
                    if (re.IsMatch(word)) return false;
                }
            }

            return word != word.ToUpper() && !containsDigit(word);
        }

        private static bool containsDigit(string text)
        {
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }
    }
}