using System;

namespace AgentSmith.MemberMatch
{
    [Flags]
    public enum ParamDirection
    {        
        In = 1,
        Out = 2,
        Ref = 4,
        Any = In | Out | Ref,
    }
}