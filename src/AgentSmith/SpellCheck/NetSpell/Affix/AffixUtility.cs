#region Copyright

/*This file is modified version of Paul Welter's one and 
* following license applies to it:
* 
* 
* Copyright (c) 2003, Paul Welter
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the <organization> nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY <copyright holder> ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/

#endregion Copyright

using System;

namespace AgentSmith.SpellCheck.NetSpell.Affix
{
    /// <summary>
    /// Summary description for <see cref="AffixUtility"/>.
    /// </summary>
    public sealed class AffixUtility
    {
        /// <summary>
        /// Adds a prefix to a word.
        /// </summary>
        /// <param name="word">
        /// The word to add the prefix to.        
        /// </param>
        /// <param name="rule">        
        /// The <see cref="AffixRule"/> to use when adding the prefix.       
        /// </param>
        /// <returns>
        /// The word with the prefix added.
        /// </returns>
        public static string AddPrefix(string word, AffixRule rule)
        {
            foreach (AffixEntry entry in rule.AffixEntries)
            {
                // check that this entry is valid
                if (word.Length >= entry.ConditionCount)
                {
                    int passCount = 0;
                    for (int i = 0; i < entry.ConditionCount; i++)
                    {
                        int charCode = word[i];
                        if ((entry.Condition[charCode] & (1 << i)) == (1 << i))
                        {
                            passCount++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (passCount == entry.ConditionCount)
                    {
                        string tempWord = word.Substring(entry.StripCharacters.Length);
                        tempWord = entry.AddCharacters + tempWord;
                        return tempWord;
                    }
                }
            }
            return word;
        }

        /// <summary>
        /// Adds a suffix to a word.
        /// </summary>
        /// <param name="word">
        ///  The word to get the suffix added to.        
        /// </param>
        /// <param name="rule">        
        ///  The <see cref="AffixRule"/> to use when adding the suffix.        
        /// </param>
        /// <returns>
        ///  The word with the suffix added.
        /// </returns>
        public static string AddSuffix(string word, AffixRule rule)
        {
            foreach (AffixEntry entry in rule.AffixEntries)
            {
                // check that this entry is valid
                if (word.Length >= entry.ConditionCount)
                {
                    int passCount = 0;
                    for (int i = 0; i < entry.ConditionCount; i++)
                    {
                        int charCode = word[word.Length - (entry.ConditionCount - i)];
                        if ((entry.Condition[charCode] & (1 << i)) == (1 << i))
                        {
                            passCount++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (passCount == entry.ConditionCount)
                    {
                        int tempLen = word.Length - entry.StripCharacters.Length;
                        string tempWord = word.Substring(0, tempLen);
                        tempWord += entry.AddCharacters;
                        return tempWord;
                    }
                }
            }
            return word;
        }

        /// <summary>
        /// Generates the condition character array.
        /// </summary>
        /// <param name="conditionText" type="string">
        /// The text form of the conditions.        
        /// </param>
        /// <param name="entry">
        /// The <see cref="AffixEntry"/> to add the condition array to.        
        /// </param>
        public static void EncodeConditions(string conditionText, AffixEntry entry)
        {
            // clear the conditions array
            for (int i = 0; i < entry.Condition.Length; i++)
            {
                entry.Condition[i] = 0;
            }

            // if no condition just return
            if (conditionText == ".")
            {
                entry.ConditionCount = 0;
                return;
            }

            bool neg = false; /* complement indicator */
            bool group = false; /* group indicator */
            bool end = false; /* end condition indicator */
            int num = 0; /* number of conditions */

            char[] memberChars = new char[200];
            int numMember = 0; /* number of member in group */

            foreach (char cond in conditionText)
            {
                // parse member group
                if (cond == '[')
                {
                    group = true; // start a group
                }
                else if (cond == '^' && group)
                {
                    neg = true; // negative group
                }
                else if (cond == ']')
                {
                    end = true; // end of a group
                }
                else if (group)
                {
                    // add chars to group
                    memberChars[numMember] = cond;
                    numMember++;
                }
                else
                {
                    end = true; // no group
                }

                // set condition
                if (end)
                {
                    if (group)
                    {
                        if (neg)
                        {
                            // turn all chars on
                            for (int j = 0; j < entry.Condition.Length; j++)
                            {
                                entry.Condition[j] = entry.Condition[j] | (1 << num);
                            }
                            // turn off chars in member group
                            for (int j = 0; j < numMember; j++)
                            {
                                int charCode = memberChars[j];
                                entry.Condition[charCode] = entry.Condition[charCode] & ~(1 << num);
                            }
                        }
                        else
                        {
                            // turn on chars in member group
                            for (int j = 0; j < numMember; j++)
                            {
                                int charCode = memberChars[j];
                                entry.Condition[charCode] = entry.Condition[charCode] | (1 << num);
                            }
                        }
                        group = false;
                        neg = false;
                        numMember = 0;
                    } // if group
                    else
                    {
                        if (cond == '.')
                        {
                            // wild card character, turn all chars on
                            for (int j = 0; j < entry.Condition.Length; j++)
                            {
                                entry.Condition[j] = entry.Condition[j] | (1 << num);
                            }
                        }
                        else
                        {
                            // turn on char
                            int charCode = cond;
                            entry.Condition[charCode] = entry.Condition[charCode] | (1 << num);
                        }
                    } // not group

                    end = false;
                    num++;
                } // if end
            } // foreach char

            entry.ConditionCount = num;
            return;
        }

        /// <summary>
        /// Removes the affix prefix rule entry for the word if valid.
        /// </summary>
        /// <param name="word">        
        /// The word to be modified.
        /// </param>
        /// <param name="entry">        
        /// The affix rule entry to use.
        /// </param>
        /// <returns>
        /// The word after affix removed.  Will be the same word if affix could not be removed.
        /// </returns>
        /// <remarks>
        ///	This method does not verify that the returned word is a valid word, only that the affix can be removed
        /// </remarks>
        public static string RemovePrefix(string word, AffixEntry entry)
        {
            int tempLength = word.Length - entry.AddCharacters.Length;
            if ((tempLength > 0)
                && (tempLength + entry.StripCharacters.Length >= entry.ConditionCount)
                && (startsWith(word, entry.AddCharacters)))
            {
                // word with out affix
                string tempWord = word.Substring(entry.AddCharacters.Length);
                // add back strip chars
                tempWord = entry.StripCharacters + tempWord;
                // check that this is valid                
                for (int i = 0; i < entry.ConditionCount; i++)
                {
                    int charCode = tempWord[i];
                    if ((entry.Condition[charCode] & (1 << i)) == 0)
                    {
                        return null;
                    }
                }
                return tempWord;
            }
            return null;
        }

        /// <summary>
        /// Removes the affix suffix rule entry for the word if valid.
        /// </summary>
        /// <param name="word">        
        /// The word to be modified.
        /// </param>
        /// <param name="entry">        
        /// The affix rule entry to use.
        /// </param>
        /// <returns>
        /// The word after affix removed.  Will be the same word if affix could not be removed.
        /// </returns>
        /// <remarks>
        ///	This method does not verify that the returned word is a valid word, only that the affix can be removed.
        /// </remarks>
        public static string RemoveSuffix(string word, AffixEntry entry)
        {
            int tempLength = word.Length - entry.AddCharacters.Length;
            if ((tempLength > 0)
                && (tempLength + entry.StripCharacters.Length >= entry.ConditionCount)
                && (endsWith(word, entry.AddCharacters)))
            {
                // word with out affix
                string tempWord = word.Substring(0, tempLength);
                // add back strip chars
                tempWord += entry.StripCharacters;
                // check that this is valid

                for (int i = 0, j = tempWord.Length - entry.ConditionCount; i < entry.ConditionCount; i++, j++)
                {
                    int charCode = tempWord[j];
                    if ((entry.Condition[charCode] & (1 << i)) == 0)
                    {
                        return null;
                    }
                }

                return tempWord;
            }
            return null;
        }

        private static bool endsWith(string s, string suffix)
        {
            if (s.Length < suffix.Length)
            {
                return false;
            }
            for (int i = s.Length - suffix.Length, j = 0; j < suffix.Length; j++, i++)
            {
                if (s[i] != suffix[j])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool startsWith(string s, string prefix)
        {
            if (s.Length < prefix.Length)
            {
                return false;
            }
            for (int i = 0; i < prefix.Length; i++)
            {
                if (s[i] != prefix[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}