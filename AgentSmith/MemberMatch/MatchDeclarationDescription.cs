using System;
using System.Collections.Generic;

namespace AgentSmith.MemberMatch
{
    public class MatchDeclarationDescription
    {
        private static readonly Dictionary<Declaration, DeclarationDescription> _dict =
            new Dictionary<Declaration, DeclarationDescription>();

        static MatchDeclarationDescription()
        {
            add(Declaration.Any);
            add(Declaration.Class);
            add(Declaration.Constant);
            add(Declaration.Delegate);
            add(Declaration.Enum);
            add(Declaration.EnumerationMember);
            add(Declaration.Event);
            add(Declaration.Field);
            add(Declaration.Interface);
            add(Declaration.Method);
            add(Declaration.Namespace);
            add(Declaration.Parameter);
            add(Declaration.Property);
            add(Declaration.Struct);
            add(Declaration.Variable);
        }

        public static Dictionary<Declaration, DeclarationDescription> DeclDescriptions
        {
            get { return _dict; }
        }

        private static void add(Declaration any)
        {
            _dict.Add(any, DeclarationDescription.DeclDescriptions[any]);
        }
    }
}