using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;

namespace AgentSmith.Comments {
	[RegisterConfigurableSeverity(
		SEVERITY_ID,
		null,
		HighlightingGroupIds.CodeSmell,
		"Word is not in the dictionary",
		"Word is not in the dictionary",
		Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(SEVERITY_ID,CSharpLanguage.Name)]
    public class WordIsNotInDictionaryHighlight : SpellCheckHighlightBase
    {
        private const string SEVERITY_ID = "WordIsNotInDictionary";
        private readonly string _word;
        private readonly LexerToken _token;

        public WordIsNotInDictionaryHighlight(string word, DocumentRange range,
                                               LexerToken misspelledToken, ISolution solution, ISpellChecker spellChecker, IContextBoundSettingsStore settingsStore)
            : base(range, misspelledToken.Value, solution, spellChecker, settingsStore)
        {
            _word = word;
            _token = misspelledToken;
        }

        public string Word
        {
            get { return _word; }
        }

        public LexerToken Token
        {
            get { return _token; }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}