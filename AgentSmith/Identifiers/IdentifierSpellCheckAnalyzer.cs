using System.Collections.Generic;

using AgentSmith.Options;
using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Naming.Interfaces;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith.Identifiers
{
    public class IdentifierSpellCheckAnalyzer
    {
        private readonly ISolution _solution;
        private readonly ISpellChecker _identifierSpellChecker;

        private readonly IContextBoundSettingsStore _settingsStore;
        private readonly IdentifierSettings _identifierSettings;

        private readonly INamingPolicyProvider _policyProvider;

        //private const int MAX_LENGTH_TO_SKIP = 0;

        public IdentifierSpellCheckAnalyzer(ISolution solution, IContextBoundSettingsStore settingsStore, IPsiSourceFile file)
        {
            _identifierSettings = settingsStore.GetKey<IdentifierSettings>(SettingsOptimization.OptimizeDefault);
            _settingsStore = settingsStore;
            _solution = solution;

            _policyProvider = file.GetPsiServices().Naming.Policy.GetPolicyProvider(CSharpLanguage.Instance, file, _settingsStore);

            _identifierSpellChecker = SpellCheckManager.GetSpellChecker(
                _settingsStore,
                _solution,
                _identifierSettings.DictionaryName == null
                    ? null
                    : _identifierSettings.DictionaryNames
                );
        }
        
        private bool IsAbbreviation(string word)
        {
            return _policyProvider.IsAbbreviation(word.ToUpper(), _solution);
        }

        public void CheckMemberSpelling(IDeclaration declaration, DefaultHighlightingConsumer consumer, bool spellCheck)
        {
            if (this._identifierSpellChecker == null || !spellCheck) return;

            if (declaration is IIndexerDeclaration ||
                declaration is IDestructorDeclaration ||
                declaration is IAccessorDeclaration ||
                declaration is IConstructorDeclaration ||
                (declaration.DeclaredName.Contains(".") && !(declaration is INamespaceDeclaration)))
            {
                return;
            }

            /*if (ComplexMatchEvaluator.IsMatch(declaration, _settings.IdentifiersToSpellCheck,
                    _settings.IdentifiersNotToSpellCheck, true) == null)
            {
                return null;
            }*/

            HashSet<string> localNames = getLocalNames(declaration);

            CamelHumpLexer lexer =
                new CamelHumpLexer(declaration.DeclaredName, 0, declaration.DeclaredName.Length);

            foreach (LexerToken token in lexer)
            {
                string val = token.Value;
                string lowerVal = val.ToLower();
                //val.Length > MAX_LENGTH_TO_SKIP &&
                if (
                    !IsAbbreviation(val) &&
                    SpellCheckUtil.ShouldSpellCheck(val, _identifierSettings.CompiledWordsToIgnore) &&
                    !localNames.Contains(lowerVal) &&
                    !this._identifierSpellChecker.TestWord(val, false))
                {
                    bool found = false;
                    foreach (string entry in localNames)
                    {
                        if (entry.StartsWith(lowerVal))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
	                    var containingFile = declaration.GetContainingFile();
	                    consumer.AddHighlighting(
		                    new IdentifierSpellCheckHighlighting(
			                    declaration,
			                    token,
			                    _solution,
			                    this._identifierSpellChecker,
			                    _settingsStore),
		                    containingFile.TranslateRangeForHighlighting(declaration.GetNameRange()),
		                    containingFile);
                    }
                }
            }
        }

        private HashSet<string> getLocalNames(IDeclaration declaration)
        {
            HashSet<string> localNames = new HashSet<string>();
            ITypeOwner var = declaration as ITypeOwner;
            if (var != null)
            {
                string name = var.Type.GetPresentableName(declaration.Language);
                string acronym = "";
                foreach (char c in name)
                {
                    if (char.IsUpper(c))
                    {
                        acronym += c;
                    }
                }
                localNames.Add(acronym.ToLower());

                CamelHumpLexer lexer = new CamelHumpLexer(name, 0, name.Length);
                foreach (LexerToken token in lexer)
                {
                    localNames.Add(token.Value.ToLower());
                }
            }

            IClassLikeDeclaration decl = declaration as IClassLikeDeclaration;
            if (decl != null)
            {
                foreach (IDeclaredType type in decl.SuperTypes)
                {
                    string name = type.GetPresentableName(declaration.Language);
                    CamelHumpLexer lexer = new CamelHumpLexer(name, 0, name.Length);
                    foreach (LexerToken token in lexer)
                    {
                        localNames.Add(token.Value.ToLower());
                    }
                }
            }
            return localNames;
        }
    }
}
