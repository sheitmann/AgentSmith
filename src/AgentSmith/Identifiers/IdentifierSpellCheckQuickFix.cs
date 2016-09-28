using System.Collections.Generic;

using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.Util;

namespace AgentSmith.Identifiers {
	[QuickFix]
	public class IdentifierSpellCheckQuickFix : IQuickFix {
		private const uint MAX_SUGGESTION_COUNT = 5;

		private readonly IdentifierSpellCheckHighlighting _highlighting;

		public IdentifierSpellCheckQuickFix(IdentifierSpellCheckHighlighting highlighting) {
			_highlighting = highlighting;
		}

		public IEnumerable<IntentionAction> CreateBulbItems() {
			return CreateItems().ToContextActionIntentions();
		}

		public bool IsAvailable(IUserDataHolder cache) {
			return true;
		}

		private IEnumerable<IBulbAction> CreateItems() {
			var items = new List<IBulbAction>();

			ISpellChecker spellChecker = _highlighting.SpellChecker;

			if (spellChecker != null) {
				foreach (string newWord in spellChecker.Suggest(_highlighting.LexerToken.Value, MAX_SUGGESTION_COUNT)) {
					if (newWord.IndexOf(" ") > 0) {
						continue;
					}
					string declaredName = _highlighting.Declaration.DeclaredName;
					string nameWithMisspelledWordDeleted = declaredName.Remove(
						_highlighting.LexerToken.Start, _highlighting.LexerToken.Length);
					string newName = nameWithMisspelledWordDeleted.Insert(_highlighting.LexerToken.Start, newWord);

					items.Add(new RenameBulbItem(_highlighting.Declaration, newName));
				}
			}
			items.Add(new RenameBulbItem(_highlighting.Declaration));

			if (spellChecker != null) {
				foreach (CustomDictionary dict in spellChecker.CustomDictionaries) {
					items.Add(
						new AddToDictionaryBulbItem(
							_highlighting.MisspelledWord, dict.Name, _highlighting.DocumentRange, _highlighting.SettingsStore));
				}
			}
			return items;
		}
	}
}
