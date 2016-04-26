using System.Collections.Generic;
using System.Text.RegularExpressions;

using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Resources.Settings;

namespace AgentSmith.Options
{
    [SettingsKey(typeof(CodeStyleSettings), "Agent Smith Settings")]
    public class AgentSmithSettings
    {


    }


    [SettingsKey(typeof(AgentSmithSettings), "Xml Documentation Settings")]
    public class XmlDocumentationSettings
    {
        private bool _wordsToIgnoreChanged = true;

        private string _wordsToIgnore;

        private List<Regex> _cachedWordsToIgnore;

        private bool _wordsToIgnoreForMetataggingChanged = true;

        private string _wordsToIgnoreForMetatagging;

        private List<Regex> _cachedWordsToIgnoreForMetatagging;
        

        [SettingsEntry("en-US", "The dictionary/s to use for xml documentation comments in source code files (use commas to separate dictionary names)")]
        public string DictionaryName { get; set; }

        public string[] DictionaryNames
        {
            get { return DictionaryName.Split(','); }
        }

        [SettingsEntry(true, "Suppress missing comment tests if identifier inherits documentation?")]
        public bool SuppressIfBaseHasComment { get; set; }

        [SettingsEntry(80, "The maximum number of characters to allow on a line when reflowing xml documentation")]
        public int MaxCharactersPerLine { get; set; }

        [SettingsEntry("", "Regular expressions for words to ignore (separate with new lines)")]
        public string WordsToIgnore
        {
            get { return _wordsToIgnore; }
            set
            {
                _wordsToIgnoreChanged = true;
                _wordsToIgnore = value;
            }
        }

        public List<Regex> CompiledWordsToIgnore
        {
            get
            {
                if (_wordsToIgnoreChanged)
                {
                    _cachedWordsToIgnore = new List<Regex>();
                    string[] regexPatterns = _wordsToIgnore.Replace("\r", "").Split('\n');

                    foreach (string regexPattern in regexPatterns)
                    {
                        if (string.IsNullOrEmpty(regexPattern)) continue;
                        Regex re = new Regex(regexPattern);
                        _cachedWordsToIgnore.Add(re);
                    }
                    _wordsToIgnoreChanged = false;

                }
                return _cachedWordsToIgnore;
            }
        }

        [SettingsEntry("", "Regular expressions for identifiers to ignore as for metatags (separate with new lines)")]
        public string WordsToIgnoreForMetatagging
        {
            get { return _wordsToIgnoreForMetatagging; }
            set
            {
                _wordsToIgnoreForMetataggingChanged = true;
                _wordsToIgnoreForMetatagging = value;
            }
        }

        public List<Regex> CompiledWordsToIgnoreForMetatagging
        {
            get
            {
                if (_wordsToIgnoreForMetataggingChanged)
                {
                    _cachedWordsToIgnoreForMetatagging = new List<Regex>();
                    string[] regexPatterns = _wordsToIgnoreForMetatagging.Replace("\r", "").Split('\n');

                    foreach (string regexPattern in regexPatterns)
                    {
                        if (string.IsNullOrEmpty(regexPattern)) continue;
                        Regex re = new Regex(regexPattern);
                        _cachedWordsToIgnoreForMetatagging.Add(re);
                    }
                    _wordsToIgnoreForMetataggingChanged = false;
                }
                return this._cachedWordsToIgnoreForMetatagging;
            }
        }

    }

    public enum WhitespaceTriState
    {
        Always = 0,
        WhenMultiLine,
        Never
    }

    [SettingsKey(typeof(XmlDocumentationSettings), "Reflow And Retag Settings")]
    public class ReflowAndRetagSettings
    {
        [SettingsIndexedEntry("Whitespace newline settings")]
        public IIndexedEntry<string, int> WhitespaceNewlineSettings;

        [SettingsIndexedEntry("Whitespace indent settings")]
        public IIndexedEntry<string, bool> WhitespaceIndentSettings;

        private int GetWhitespaceSetting(string settingName, int defaultValue = 0)
        {
            int result;

            return WhitespaceNewlineSettings.TryGet(settingName, out result) ? result : defaultValue;
        }

        public WhitespaceTriState SummaryTagOnNewLine
        {
            get { return (WhitespaceTriState) GetWhitespaceSetting("SummaryTagOnNewLine"); }
        }
        public WhitespaceTriState RemarksTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("RemarksTagOnNewLine"); }
        }
        public WhitespaceTriState ExampleTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("ExampleTagOnNewLine"); }
        }
        public WhitespaceTriState ReturnsTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("ReturnsTagOnNewLine"); }
        }
        public WhitespaceTriState ParaTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("ParaTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState ParamTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("ParamTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState TypeParamTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("TypeParamTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState ListTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("ListTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState ListHeaderTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("ListHeaderTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState ItemTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("ItemTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState TermTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("TermTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState DescriptionTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("DescriptionTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState ExceptionTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("ExceptionTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }
        public WhitespaceTriState PermissionTagOnNewLine
        {
            get { return (WhitespaceTriState)GetWhitespaceSetting("PermissionTagOnNewLine", (int)WhitespaceTriState.WhenMultiLine); }
        }



        private bool GetIndentSetting(string settingName, bool defaultValue = false)
        {
            bool result;

            return WhitespaceIndentSettings.TryGet(settingName, out result) ? result : defaultValue;
        }

        public bool SummaryTagIndent
        {
            get { return GetIndentSetting("SummaryTagIndent"); }
        }
        public bool RemarksTagIndent
        {
            get { return GetIndentSetting("RemarksTagIndent"); }
        }
        public bool ExampleTagIndent
        {
            get { return GetIndentSetting("ExampleTagIndent"); }
        }
        public bool ReturnsTagIndent
        {
            get { return GetIndentSetting("ReturnsTagIndent"); }
        }
        public bool ParaTagIndent
        {
            get { return GetIndentSetting("ParaTagIndent"); }
        }
        public bool ParamTagIndent
        {
            get { return GetIndentSetting("ParamTagIndent"); }
        }
        public bool TypeParamTagIndent
        {
            get { return GetIndentSetting("TypeParamTagIndent"); }
        }
        public bool ListTagIndent
        {
            get { return GetIndentSetting("ListTagIndent"); }
        }
        public bool ListHeaderTagIndent
        {
            get { return GetIndentSetting("ListHeaderTagIndent"); }
        }
        public bool ItemTagIndent
        {
            get { return GetIndentSetting("ItemTagIndent"); }
        }
        public bool TermTagIndent
        {
            get { return GetIndentSetting("TermTagIndent"); }
        }
        public bool DescriptionTagIndent
        {
            get { return GetIndentSetting("DescriptionTagIndent"); }
        }
        public bool ExceptionTagIndent
        {
            get { return GetIndentSetting("ExceptionTagIndent"); }
        }
        public bool PermissionTagIndnent
        {
            get { return GetIndentSetting("PermissionTagIndnent"); }
        }

    }

    public enum IdentifierLookupScopes
    {
        ProjectOnly,
        ProjectAndUsings,
        ProjectAndReferencedLibraries,
        ProjectAndAllLibraries
    }

    public static class IdentifierLookupScopesEx
    {
		public static LibrarySymbolScope AsLibrarySymbolScope(this IdentifierLookupScopes scope) {
			switch (scope) {
				case IdentifierLookupScopes.ProjectOnly:
					return LibrarySymbolScope.NONE; //DeclarationCacheLibraryScope.NONE;
				case IdentifierLookupScopes.ProjectAndUsings:
					return LibrarySymbolScope.TRANSITIVE; // DeclarationCacheLibraryScope.TRANSITIVE;
				case IdentifierLookupScopes.ProjectAndReferencedLibraries:
					return LibrarySymbolScope.REFERENCED; //DeclarationCacheLibraryScope.REFERENCED;
				case IdentifierLookupScopes.ProjectAndAllLibraries:
					return LibrarySymbolScope.FULL; //DeclarationCacheLibraryScope.FULL;
			}
			return LibrarySymbolScope.NONE; //DeclarationCacheLibraryScope.NONE;
		}
    }

    [SettingsKey(typeof(AgentSmithSettings), "Identifier Settings")]
    public class IdentifierSettings
    {
        private bool _wordsToIgnoreChanged = true;

        private string _wordsToIgnore;

        private List<Regex> _cachedWordsToIgnore;

        [SettingsEntry("en-US", "The dictionary/s to use for identifiers in source code files (use commas to separate dictionary names)")]
        public string DictionaryName { get; set; }

        public string[] DictionaryNames
        {
            get { return DictionaryName.Split(','); }
        }

        [SettingsEntry(1, "Scope for searching for identifiers")]
        public int LookupScope { get; set; }

        public IdentifierLookupScopes IdentifierLookupScope
        {
            get
            {
                switch (LookupScope)
                {
                    case 0:
                        return IdentifierLookupScopes.ProjectOnly;
                    case 1:
                        return IdentifierLookupScopes.ProjectAndUsings;
                    case 2:
                        return IdentifierLookupScopes.ProjectAndReferencedLibraries;
                    case 3:
                        return IdentifierLookupScopes.ProjectAndAllLibraries;
                }
                return IdentifierLookupScopes.ProjectAndUsings;
            }
        }

        [SettingsEntry("", "Regular expressions for identifiers to ignore during spell checking (separate with new lines)")]
        public string WordsToIgnore
        {
            get { return this._wordsToIgnore; }
            set
            {
                this._wordsToIgnoreChanged = true;
                this._wordsToIgnore = value;
            }
        }

        public List<Regex> CompiledWordsToIgnore
        {
            get
            {
                if (_wordsToIgnoreChanged)
                {
                    _cachedWordsToIgnore = new List<Regex>();
                    string[] regexPatterns = _wordsToIgnore.Replace("\r", "").Split('\n');

                    foreach (string regexPattern in regexPatterns)
                    {
                        if (string.IsNullOrEmpty(regexPattern)) continue;
                        Regex re = new Regex(regexPattern);
                        _cachedWordsToIgnore.Add(re);
                    }
                    _wordsToIgnoreChanged = false;
                }
                return _cachedWordsToIgnore;
            }
        }


    }

    [SettingsKey(typeof(AgentSmithSettings), "String Literal Settings")]
    public class StringSettings
    {
        private bool _wordsToIgnoreChanged = true;

        private string _wordsToIgnore;

        private List<Regex> _cachedWordsToIgnore;

        [SettingsEntry("en-US", "The dictionary/s to use for string literals in source code files (use commas to separate dictionary names)")]
        public string DictionaryName { get; set; }

        public string[] DictionaryNames
        {
            get { return DictionaryName.Split(','); }
        }

        [SettingsEntry(true, "Spell check verbatim strings?")]
        public bool IgnoreVerbatimStrings { get; set; }

        [SettingsEntry("", "Regular expressions for words to ignore (separate with new lines)")]
        public string WordsToIgnore
        {
            get { return _wordsToIgnore; }
            set
            {
                _wordsToIgnoreChanged = true;
                _wordsToIgnore = value;
            }
        }

        public List<Regex> CompiledWordsToIgnore
        {
            get
            {
                if (_wordsToIgnoreChanged)
                {
                    _cachedWordsToIgnore = new List<Regex>();
                    string[] regexPatterns = _wordsToIgnore.Replace("\r", "").Split('\n');

                    foreach (string regexPattern in regexPatterns)
                    {
                        if (string.IsNullOrEmpty(regexPattern)) continue;
                        Regex re = new Regex(regexPattern);
                        _cachedWordsToIgnore.Add(re);
                    }
                    _wordsToIgnoreChanged = false;
                }
                return _cachedWordsToIgnore;
            }
        }
    }

    [SettingsKey(typeof(AgentSmithSettings), "Inline Comment Settings")]
    public class CommentSettings
    {
        private bool _wordsToIgnoreChanged = true;

        private string _wordsToIgnore;

        private List<Regex> _cachedWordsToIgnore;

        [SettingsEntry("en-US", "The dictionary/s to use for comments in source code files (use commas to separate dictionary names)")]
        public string DictionaryName { get; set; }

        public string[] DictionaryNames
        {
            get { return DictionaryName.Split(','); }
        }

        [SettingsEntry("", "Regular expressions for words to ignore (separate with new lines)")]
        public string WordsToIgnore
        {
            get { return _wordsToIgnore; }
            set
            {
                _wordsToIgnoreChanged = true;
                _wordsToIgnore = value;
            }
        }

        public List<Regex> CompiledWordsToIgnore
        {
            get
            {
                if (_wordsToIgnoreChanged)
                {
                    _cachedWordsToIgnore = new List<Regex>();
                    string[] regexPatterns = _wordsToIgnore.Replace("\r", "").Split('\n');

                    foreach (string regexPattern in regexPatterns)
                    {
                        if (string.IsNullOrEmpty(regexPattern)) continue;
                        Regex re = new Regex(regexPattern);
                        _cachedWordsToIgnore.Add(re);
                    }
                    _wordsToIgnoreChanged = false;
                }
                return _cachedWordsToIgnore;
            }
        }
    }


    [SettingsKey(typeof(AgentSmithSettings), "Resource File Settings")]
    public class ResXSettings
    {
        private bool _wordsToIgnoreChanged = true;

        private string _wordsToIgnore;

        private List<Regex> _cachedWordsToIgnore;

        [SettingsEntry("en-US", "The additional (to the resX language) dictionary/s to use for string literals in source code files (use commas to separate dictionary names)")]
        public string DictionaryName { get; set; }

        public string[] DictionaryNames
        {
            get { return DictionaryName.Split(','); }
        }

        [SettingsEntry("", "Regular expressions for words to ignore (separate with new lines)")]
        public string WordsToIgnore
        {
            get { return _wordsToIgnore; }
            set
            {
                _wordsToIgnoreChanged = true;
                _wordsToIgnore = value;
            }
        }

        public List<Regex> CompiledWordsToIgnore
        {
            get
            {
                if (_wordsToIgnoreChanged)
                {
                    _cachedWordsToIgnore = new List<Regex>();
                    string[] regexPatterns = _wordsToIgnore.Replace("\r", "").Split('\n');
                    
                    foreach (string regexPattern in regexPatterns)
                    {
                        if (string.IsNullOrEmpty(regexPattern)) continue;

                        Regex re = new Regex(regexPattern);
                        _cachedWordsToIgnore.Add(re);
                    }
                    _wordsToIgnoreChanged = false;
                }
                return _cachedWordsToIgnore;
            }
        }
    }


    [SettingsKey(typeof(AgentSmithSettings), "User Dictionaries")]
    public class CustomDictionarySettings
    {
        [SettingsIndexedEntry("User Dictionaries")]
        public IIndexedEntry<string, CustomDictionary> CustomDictionaries { get; set; }

    }

}


