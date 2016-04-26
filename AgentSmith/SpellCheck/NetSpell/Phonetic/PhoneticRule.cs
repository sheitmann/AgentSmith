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

namespace AgentSmith.SpellCheck.NetSpell.Phonetic
{
    /// <summary>
    ///	This class hold the settings for a phonetic rule.
    /// </summary>
    public class PhoneticRule
    {
        private readonly int[] _condition = new int[256];
        private bool _beginningOnly;
        private int _conditionCount = 0;
        private int _consumeCount;
        private bool _endOnly;
        private int _priority;
        private bool _replaceMode = false;
        private string _replaceString;

        /// <summary>
        /// True if this rule should be applied to the beginning only.
        /// </summary>
        public bool BeginningOnly
        {
            get { return _beginningOnly; }
            set { _beginningOnly = value; }
        }

        /// <summary>
        /// The ASCII condition array.
        /// </summary>
        public int[] Condition
        {
            get { return _condition; }
        }

        /// <summary>
        /// The number of conditions.
        /// </summary>
        public int ConditionCount
        {
            get { return _conditionCount; }
            set { _conditionCount = value; }
        }

        /// <summary>
        /// The number of chars to consume with this rule.
        /// </summary>
        public int ConsumeCount
        {
            get { return _consumeCount; }
            set { _consumeCount = value; }
        }

        /// <summary>
        /// True if this rule should be applied to the end only.
        /// </summary>
        public bool EndOnly
        {
            get { return _endOnly; }
            set { _endOnly = value; }
        }

        /// <summary>
        /// The priority of this rule.
        /// </summary>
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        /// <summary>
        /// True if this rule should run in replace mode.
        /// </summary>
        public bool ReplaceMode
        {
            get { return _replaceMode; }
            set { _replaceMode = value; }
        }

        /// <summary>
        /// The string to use when replacing.
        /// </summary>
        public string ReplaceString
        {
            get { return _replaceString; }
            set { _replaceString = value; }
        }
    }
}