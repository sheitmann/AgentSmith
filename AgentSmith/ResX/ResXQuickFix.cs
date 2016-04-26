using System.Collections.Generic;

using AgentSmith.SpellCheck;
using AgentSmith.SpellCheck.NetSpell;

using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.Util;

namespace AgentSmith.ResX {
	[QuickFix]
	public class ResXQuickFix : IQuickFix {
		private const int MAX_SUGGESTIONS = 5;

		private readonly ResXSpellHighlighting _highlighting;

		public ResXQuickFix(ResXSpellHighlighting highlighting) {
			_highlighting = highlighting;
		}

		#region IQuickFix Members

		public IEnumerable<IntentionAction> CreateBulbItems() {
			return CreateItems()
				.ToContextAction();
		}

		public bool IsAvailable(IUserDataHolder cache) {
			return true;
		}

		private IEnumerable<IBulbAction> CreateItems() {
			var items = new List<IBulbAction>();
			ISpellChecker spellChecker = _highlighting.SpellChecker;
			foreach (string suggestion in spellChecker.Suggest(_highlighting.MisspelledWord, MAX_SUGGESTIONS)) {
				items.Add(new ReplaceWordWithBulbItem(_highlighting.DocumentRange, suggestion));
			}
			foreach (CustomDictionary dict in _highlighting.SpellChecker.CustomDictionaries) {
				items.Add(
					new AddToDictionaryBulbItem(
						_highlighting.MisspelledWord, dict.Name, _highlighting.DocumentRange, _highlighting.SettingsStore));
			}
			return items;
		}

		#endregion
	}
}