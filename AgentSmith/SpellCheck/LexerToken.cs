namespace AgentSmith.SpellCheck
{
    public struct LexerToken
    {
        public string Buffer;
        public int End;
        public int Start;

        public LexerToken(string buffer, int start, int end)
        {
            Buffer = buffer;
            Start = start;
            End = end;
        }

        public string Value
        {
            get { return Buffer.Substring(Start, Length); }
        }

        public int Length
        {
            get { return End - Start; }
        }
    }
}