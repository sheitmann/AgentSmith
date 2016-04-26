using AgentSmith.Identifiers;
using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

[assembly: RegisterConfigurableSeverity(
    IdentifierSpellCheckHighlighting.SEVERITY_ID,
    null,
    HighlightingGroupIds.CodeSmell,
    "Spelling errors in identifiers",
    "Spelling errors in identifiers",
    Severity.SUGGESTION,
    false)]

namespace AgentSmith.Identifiers
{
    [ConfigurableSeverityHighlighting(SEVERITY_ID, CSharpLanguage.Name)]
    public class IdentifierSpellCheckHighlighting : SpellCheckHighlightBase
    {
        public const string SEVERITY_ID = "IdentifierWordIsNotInDictionary";

        private readonly IDeclaration _declaration;
        private readonly LexerToken _lexerToken;

        public IdentifierSpellCheckHighlighting(IDeclaration declaration, LexerToken token,
                                              ISolution solution, ISpellChecker spellChecker, IContextBoundSettingsStore settingsStore)
            : base(GetRange(declaration), token.Value, solution, spellChecker, settingsStore)
        {
            _lexerToken = token;
            _declaration = declaration;
        }

        public IDeclaration Declaration
        {
            get { return _declaration; }
        }

        public LexerToken LexerToken
        {
            get { return _lexerToken; }
        }

        private static DocumentRange GetRange(IDeclaration declaration)
        {
            INamespaceDeclaration namespaceDeclaration = declaration as INamespaceDeclaration;
            if (namespaceDeclaration != null)
            {
                return namespaceDeclaration.GetDeclaredNameDocumentRange();
            }
            return declaration.GetNameDocumentRange();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}