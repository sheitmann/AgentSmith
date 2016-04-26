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

namespace AgentSmith.SpellCheck.NetSpell
{
    /// <summary>
    /// The Word class represents a base word in the dictionary.
    /// </summary>
    public class Word : IComparable
    {
        private string _affixKeys = "";
        private int _editDistance = 0;
        private int _height = 0;
        private int _index = 0;
        private string _phoneticCode = "";
        private string _text = "";

        public Word()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="text" type="string">        
        /// The string for the base word.        
        /// </param>
        /// <param name="affixKeys" type="string">        
        /// The affix keys that can be applied to this base word.        
        /// </param>
        /// <param name="phoneticCode" type="string">        
        /// The phonetic code for this word.        
        /// </param>
        public Word(string text, string affixKeys, string phoneticCode)
        {
            _text = text;
            _affixKeys = affixKeys;
            _phoneticCode = phoneticCode;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="text" type="string">        
        /// The string for the base word.        
        /// </param>
        /// <param name="affixKeys" type="string">        
        ///  The affix keys that can be applied to this base word.        
        /// </param>
        public Word(string text, string affixKeys)
        {
            _text = text;
            _affixKeys = affixKeys;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="text" type="string">        
        /// The string for the base word.        
        /// </param>
        public Word(string text)
        {
            _text = text;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="text" type="string">        
        /// The string for the word.        
        /// </param>
        /// <param name="index" type="int">        
        /// The position index of this word.        
        /// </param>
        /// <param name="height" type="int">        
        ///  The line height of this word.        
        /// </param>
        /// <returns>
        ///  A void value...
        /// </returns>
        internal Word(string text, int index, int height)
        {
            _text = text;
            _index = index;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="text" type="string">        
        ///  The string for the base word.        
        /// </param>
        /// <param name="editDistance" type="int">        
        ///  The edit distance from the misspelled word        
        /// </param>
        internal Word(string text, int editDistance)
        {
            _text = text;
            _editDistance = editDistance;
        }

        /// <summary>
        /// The affix keys that can be applied to this base word.
        /// </summary>
        public string AffixKeys
        {
            get { return _affixKeys; }
            set { _affixKeys = value; }
        }

        /// <summary>
        /// The index position of where this word appears.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// The phonetic code for this word.
        /// </summary>
        public string PhoneticCode
        {
            get { return _phoneticCode; }
            set { _phoneticCode = value; }
        }

        /// <summary>
        /// The string for the base word.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Used for sorting suggestions by its edit distance for 
        /// the misspelled word.
        /// </summary>
        internal int EditDistance
        {
            get { return _editDistance; }
            set { _editDistance = value; }
        }

        /// <summary>
        /// The line height of this word.
        /// </summary>
        internal int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        #region IComparable Members

        /// <summary>
        ///     Sorts a collection of words by <see cref="EditDistance"/>.
        /// </summary>
        /// <remarks>
        ///		The compare sorts in descending order, largest <see cref="EditDistance"/> first.
        /// </remarks>
        public int CompareTo(object obj)
        {
            int result = EditDistance.CompareTo(((Word)obj).EditDistance);
            return result; // * -1; // sorts desc order
        }

        #endregion

        /// <summary>
        /// Converts the word object to a string.
        /// </summary>
        /// <returns>
        /// Returns the Text Property contents.
        /// </returns>
        public override string ToString()
        {
            return _text;
        }
    }
}