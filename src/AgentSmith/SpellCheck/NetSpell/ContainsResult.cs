using System;

namespace AgentSmith.SpellCheck.NetSpell
{
    public struct ContainsResult
    {
        public readonly bool Contains;
        public readonly string[] PossibleBaseWords;

        public ContainsResult(bool contains, string[] possibleBaseWords)
        {
            Contains = contains;
            PossibleBaseWords = possibleBaseWords;
        }
    }
}