using System;
using System.Collections.Generic;

using AgentSmith.SpellCheck;

using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.Util;

namespace AgentSmith.Comments {
	[QuickFix]
	public class SurroundWithMetatagsQuickFix : IQuickFix {
		private readonly CanBeSurroundedWithMetatagsHighlight _suggestion;

		public SurroundWithMetatagsQuickFix(CanBeSurroundedWithMetatagsHighlight suggestion) {
			_suggestion = suggestion;
		}

		public IEnumerable<IntentionAction> CreateBulbItems() {
			return CreateItems().ToContextActionIntentions();
		}

		public bool IsAvailable(IUserDataHolder cache) {
			return true;
		}

		private IEnumerable<IBulbAction> CreateItems() {
			var items = new List<IBulbAction>();

			IList<string> replaceFormats = IdentifierResolver.GetReplaceFormats(
				_suggestion.Declaration, _suggestion.Solution, _suggestion.Word);

			foreach (string format in replaceFormats) {
				string replacement = String.Format(format, _suggestion.Word);
				items.Add(new ReplaceWordWithBulbItem(_suggestion.DocumentRange, replacement));
			}

			items.Add(new ReplaceWordWithBulbItem(_suggestion.DocumentRange, String.Format("<c>{0}</c>", _suggestion.Word)));

			return items;
		}
	}
}
