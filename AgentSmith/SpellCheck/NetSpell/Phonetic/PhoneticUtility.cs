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
using System.Globalization;

namespace AgentSmith.SpellCheck.NetSpell.Phonetic
{
    /// <summary>
    ///	This class holds helper methods for phonetic encoding.
    /// </summary>
    public sealed class PhoneticUtility
    {
        /// <summary>
        /// Converts the rule text in to a <see cref="PhoneticRule"/> class.
        /// </summary>
        /// <param name="ruleText" type="string">
        /// The text to convert.
        /// </param>
        /// <param name="rule" type="ref NetSpell.SpellChecker.Dictionary.Phonetic.PhoneticRule">
        /// The object that will hold the conversion data.        
        /// </param>
        public static void EncodeRule(string ruleText, ref PhoneticRule rule)
        {
            // clear the conditions array
            for (int i = 0; i < rule.Condition.Length; i++)
            {
                rule.Condition[i] = 0;
            }

            bool group = false; /* group indicator */
            bool end = false; /* end condition indicator */

            char[] memberChars = new char[200];
            int numMember = 0; /* number of member in group */

            foreach (char cond in ruleText)
            {
                switch (cond)
                {
                    case '(':
                        group = true;
                        break;
                    case ')':
                        end = true;
                        break;
                    case '^':
                        rule.BeginningOnly = true;
                        break;
                    case '$':
                        rule.EndOnly = true;
                        break;
                    case '-':
                        rule.ConsumeCount++;
                        break;
                    case '<':
                        rule.ReplaceMode = true;
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        rule.Priority = int.Parse(cond.ToString(CultureInfo.CurrentUICulture));
                        break;
                    default:
                        if (group)
                        {
                            // add chars to group
                            memberChars[numMember] = cond;
                            numMember++;
                        }
                        else
                        {
                            end = true;
                        }
                        break;
                } // switch

                if (end)
                {
                    if (group)
                    {
                        // turn on chars in member group
                        for (int j = 0; j < numMember; j++)
                        {
                            int charCode = memberChars[j];
                            rule.Condition[charCode] = rule.Condition[charCode] | (1 << rule.ConditionCount);
                        }

                        group = false;
                        numMember = 0;
                    }
                    else
                    {
                        // turn on char
                        int charCode = cond;
                        rule.Condition[charCode] = rule.Condition[charCode] | (1 << rule.ConditionCount);
                    }
                    end = false;
                    rule.ConditionCount++;
                } // if end
            } // for each
        }
    }
}