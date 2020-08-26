using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace AgentSmith.ResX {
	[RegisterConfigurableSeverity(
		NAME,
		null,
		HighlightingGroupIds.CodeSmell,
		"Spelling errors in resx",
		"Spelling errors in resx",
		Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(NAME, CSharpLanguage.Name)]
    public class ResXSpellHighlighting : SpellCheckHighlightBase
    {
        private const string NAME = "ResxSpellCheckSuggestion";
        private readonly IPsiSourceFile _file;

        public ResXSpellHighlighting(string word, IPsiSourceFile file, ISpellChecker spellChecker, DocumentRange range, IContextBoundSettingsStore settingsStore)
            : base(range, word, file.GetSolution(), spellChecker, settingsStore)
        {
            _file = file;
        }

        public IPsiSourceFile File
        {
            get { return _file; }
        }
    }
}