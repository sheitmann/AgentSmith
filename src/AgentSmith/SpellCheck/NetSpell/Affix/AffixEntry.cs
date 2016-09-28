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
    /// Rule Entry for expanding base words.
    /// </summary>
    public class AffixEntry
    {
        /// <summary>
        ///  The characters to add to the string.
        /// </summary>
        public string AddCharacters = "";

        /// <summary>
        ///  The condition to be met in order to add characters.
        /// </summary>
        public int[] Condition = new int[256];

        /// <summary>
        ///  The number of conditions that must be met.
        /// </summary>
        public int ConditionCount;

        /// <summary>
        ///  The characters to remove before adding characters.
        /// </summary>
        public string StripCharacters = "";
    }
}