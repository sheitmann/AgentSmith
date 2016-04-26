using System;
using System.Globalization;
using System.Text;
using System.Windows.Markup;
using System.Xml.Serialization;

using JetBrains.ReSharper.Psi.JavaScript.Impl.DocComments;
using JetBrains.Util;
using JetBrains.Util.Reflection;

namespace AgentSmith.SpellCheck.NetSpell
{
    [ValueSerializer(typeof(CustomDictionarySerializer))]
    public class CustomDictionary : ICloneable
    {
        private bool _caseSensitive;
        private string _name;
        private string _userWords = "";
        private string _decodedWords;
        private int _version;
        private bool _encoded;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _version++;
            }
        }

        public bool Encoded
        {
            get { return _encoded; }
            set { _encoded = value; }
        }

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                _caseSensitive = value;
                _version++;
            }
        }

        /// <summary>
        /// Encoded string. Encoding because in R# 3.1 non ASCII characters may cause problems.
        /// </summary>
        public string UserWords
        {
            get
            {
                return _decodedWords == null ? "" : Convert.ToBase64String(Encoding.Unicode.GetBytes(_decodedWords));
            }
            set
            {
                _userWords = value;
                _decodedWords = null;
                _version++;
            }
        }

        public int Version
        {
            get { return _version; }
        }

        public string DecodedUserWords
        {
            get {
                return this._decodedWords
                       ??
                       (this._decodedWords =
                        this._encoded
                            ? Encoding.Unicode.GetString(Convert.FromBase64String(this._userWords))
                            : this._userWords);
            }
            set
            {
                _encoded = true;
                _decodedWords = value;
                _version++;
            }
        }

        public object Clone()
        {
            CustomDictionary dictionary = new CustomDictionary();
            dictionary._caseSensitive = _caseSensitive;
            dictionary._encoded = _encoded;
            dictionary._name = _name;
            dictionary._userWords = _userWords;
            dictionary._version = _version;
            return dictionary;
        }

        public override bool Equals(object obj)
        {
            CustomDictionary dict = obj as CustomDictionary;
            if (dict == null) return false;
            return Name.Equals(dict.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }


    public class CustomDictionarySerializer : ValueSerializerBase<CustomDictionary>
    {
        private const string NAME = "Name";
        private const string CASE_SENSITIVE = "CaseSensitive";
        private const string ENCODED = "Encoded";
        private const string USER_WORDS = "UserWords";

        public CustomDictionarySerializer() : base(CustomDictionaryToString, StringToCustomDictionary) { }

        private static CustomDictionary StringToCustomDictionary(string s, ILogger logger)
        {
            try
            {
                return TypeConverterUtil.FromStringThruXml(
                    s,
                    element =>
                        {
                            string name = element.ReadAttribute(NAME);
                            bool caseSensitive = false;
                            bool.TryParse(element.ReadAttribute(CASE_SENSITIVE), out caseSensitive);
                            bool encoded = false;
                            bool.TryParse(element.ReadAttribute(ENCODED), out encoded);
                            string userWords = element.ReadAttribute(USER_WORDS);

                            return new CustomDictionary()
                                { Name = name, CaseSensitive = caseSensitive, UserWords = userWords, Encoded = encoded };
                        });
            }
            catch
            {
                return null;
            }
        }

        private static string CustomDictionaryToString(CustomDictionary customDictionary, ILogger logger)
        {
            return TypeConverterUtil.ToStringThruXml(element =>
            {
                element.CreateAttributeWithNonEmptyValue(NAME, customDictionary.Name);
                element.CreateAttributeWithNonEmptyValue(CASE_SENSITIVE, customDictionary.CaseSensitive.ToString(CultureInfo.InvariantCulture));
                element.CreateAttributeWithNonEmptyValue(USER_WORDS, customDictionary.UserWords);
                element.CreateAttributeWithNonEmptyValue(ENCODED, customDictionary.Encoded.ToString(CultureInfo.InvariantCulture));
            });
        }
    }

}