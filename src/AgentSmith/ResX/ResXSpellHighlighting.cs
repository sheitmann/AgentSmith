using AgentSmith.ResX;
using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

#if RESHARPER20161 || RESHARPER20162
	[assembly: RegisterConfigurableSeverity(
	ResXSpellHighlighting.NAME,
	null,
	HighlightingGroupIds.CodeSmell,
	"Spelling errors in resx",
	"Spelling errors in resx",
	Severity.SUGGESTION,
	false)]
#else
	[assembly: RegisterConfigurableSeverity(
	ResXSpellHighlighting.NAME,
	null,
	HighlightingGroupIds.CodeSmell,
	"Spelling errors in resx",
	"Spelling errors in resx",
	Severity.SUGGESTION)]
#endif

namespace AgentSmith.ResX
{

    [ConfigurableSeverityHighlighting(NAME, CSharpLanguage.Name)]
    public class ResXSpellHighlighting : SpellCheckHighlightBase
    {
        public const string NAME = "ResxSpellCheckSuggestion";
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