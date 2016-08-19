using System;

namespace AgentSmith.MemberMatch
{
    [Flags]
    public enum AccessLevels
    {
        Any = 255,
        None = 0,
        Public = 1,
        Private = 2,
        Internal = 4,
        Protected = 8,
        ProtectedInternal = 16,
        ProtectedAndInternal = 32
    }
}