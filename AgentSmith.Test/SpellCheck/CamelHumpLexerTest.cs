using System.Collections.Generic;
using AgentSmith.SpellCheck;

using NUnit.Framework;

namespace AgentSmith.Test.SpellCheck {

	[TestFixture]
	public class CamelHumpLexerTest {
		

		[Test]
		public void Test() {
			testTokens("SimpleCase", "Simple", "Case");
			testTokens("aSimpleCase", "a", "Simple", "Case");
			testTokens("One", "One");
			testTokens("AnACRONYMCase", "An", "ACRONYM", "Case");
			testTokens("JustACRONYM", "Just", "ACRONYM");
			testTokens("lastM", "last", "M");
			testTokens("digits123AndLetters", "digits123", "And", "Letters");
			testTokens("1a", "1a");
			testTokens("_underscore_test", "underscore", "test");
			testTokens("@identifier", "identifier");

		}

		private static void testTokens(string word, params string[] tokens) {
			var list = new List<LexerToken>(new CamelHumpLexer(word, 0, word.Length));
			Assert.AreEqual(tokens.Length, list.Count, "Number of aTokens returned is different.");
			for (int i = 0; i < tokens.Length; i++) {
				Assert.AreEqual(tokens[i], list[i].Value, "Incorrect token.");
			}
		}
	}
}