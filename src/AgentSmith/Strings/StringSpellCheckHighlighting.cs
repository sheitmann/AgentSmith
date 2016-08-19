using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;
using AgentSmith.Strings;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;

[assembly: RegisterConfigurableSeverity(
    StringSpellCheckHighlighting.SEVERITY_ID,
    null,
    HighlightingGroupIds.CodeSmell,
    "String literal word is not in dictionary",
    "String literal word is not in dictionary",
    Severity.SUGGESTION,
    false)]

namespace AgentSmith.Strings
{
    [ConfigurableSeverityHighlighting(SEVERITY_ID, CSharpLanguage.Name)]
    public class StringSpellCheckHighlighting : SpellCheckHighlightBase
    {
        public const string SEVERITY_ID = "StringLiteralsWordIsNotInDictionary";

        private readonly string _word;        
        private readonly TextRange _misspelledRange;

        public StringSpellCheckHighlighting(string word, DocumentRange range, string misspelledWord,
                                          TextRange misspelledRange, ISolution solution,
                                          ISpellChecker spellChecker, IContextBoundSettingsStore settingsStore)
            : base(range, misspelledWord, solution, spellChecker, settingsStore)
        {
            _word = word;            
            _misspelledRange = misspelledRange;
        }

        public string Word
        {
            get { return _word; }
        }

        public TextRange MisspelledRange
        {
            get { return _misspelledRange; }
        }
        
        public override bool IsValid()
        {
            return true;
        }

    }
}