using System;
using System.Collections.Generic;

using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.Util;

namespace AgentSmith.Comments {
	[QuickFix]
	public class XmlCommentSyntaxQuickFix : IQuickFix {
		private const uint MAX_SUGGESTION_COUNT = 5;

		private readonly WordIsNotInDictionaryHighlight _highlight;

		public XmlCommentSyntaxQuickFix(WordIsNotInDictionaryHighlight highlight) {
			_highlight = highlight;
		}

		#region IQuickFix Members

		public IEnumerable<IntentionAction> CreateBulbItems() {
			return CreateItems().ToContextActionIntentions();
		}

		public bool IsAvailable(IUserDataHolder cache) {
			return true;
		}

		private IEnumerable<IBulbAction> CreateItems() {
			var items = new List<IBulbAction>();

			ISpellChecker spellChecker = _highlight.SpellChecker;

			if (spellChecker != null) {
				foreach (string suggestText in spellChecker.Suggest(_highlight.MisspelledWord, MAX_SUGGESTION_COUNT)) {
					string wordWithMisspelledWordDeleted = _highlight.Word.Remove(_highlight.Token.Start, _highlight.Token.Length);
					string newWord = wordWithMisspelledWordDeleted.Insert(_highlight.Token.Start, suggestText);
					items.Add(new ReplaceWordWithBulbItem(_highlight.DocumentRange, newWord));
				}
			}

			items.Add(new ReplaceWordWithBulbItem(_highlight.DocumentRange, String.Format("<c>{0}</c>", _highlight.Word)));
			if (spellChecker != null) {
				foreach (CustomDictionary customDict in spellChecker.CustomDictionaries) {
					items.Add(
						new AddToDictionaryBulbItem(
							_highlight.Token.Value, customDict.Name, _highlight.DocumentRange, _highlight.SettingsStore));
				}
			}
			return items;
		}
#endregion
	}
}