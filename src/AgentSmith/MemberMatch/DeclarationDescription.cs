using System;
using System.Collections.Generic;

namespace AgentSmith.MemberMatch
{
    public class DeclarationDescription
    {
        private static readonly Dictionary<Declaration, DeclarationDescription> _dict =
            new Dictionary<Declaration, DeclarationDescription>();

        public readonly string Name;
        public readonly Declaration Declaration;
        public readonly bool HasAccessLevel;
        public readonly bool CanInherit;
        public readonly bool CanBeMarkedWithAttribute;
        public readonly bool CanBeStatic;
        public readonly bool CanBeReadonly;
        public readonly bool OwnsType;

        static DeclarationDescription()
        {
            _dict.Add(Declaration.Any, new DeclarationDescription(Declaration.Any, "Any Member", true, false, false, false, false, false));
            _dict.Add(Declaration.Class, new DeclarationDescription(Declaration.Class, "Class", true, true, true, true, false, false));
            _dict.Add(Declaration.Constant, new DeclarationDescription(Declaration.Constant, "Constant", true, false, true, false, false, true));
            _dict.Add(Declaration.Delegate, new DeclarationDescription(Declaration.Delegate, "Delegate", true, false, true, false, false, false));
            _dict.Add(Declaration.Enum, new DeclarationDescription(Declaration.Enum, "Enumeration", true, false, true, false, false, false));
            _dict.Add(Declaration.EnumerationMember, new DeclarationDescription(Declaration.EnumerationMember, "Enumeration member", true, false, true, false, false, false));
            _dict.Add(Declaration.Event, new DeclarationDescription(Declaration.Event, "Event", true, false, true, true, false, true));
            _dict.Add(Declaration.Field, new DeclarationDescription(Declaration.Field, "Field", true, false, true, true, true, true));
            _dict.Add(Declaration.Interface, new DeclarationDescription(Declaration.Interface, "Interface", true, true, true, false, false, false));
            _dict.Add(Declaration.Method, new DeclarationDescription(Declaration.Method, "Method", true, false, true, true, false, false));
            _dict.Add(Declaration.Namespace, new DeclarationDescription(Declaration.Namespace, "Namespace", false, false, false, false, false, false));
            _dict.Add(Declaration.Parameter, new DeclarationDescription(Declaration.Parameter, "Parameter", true, false, true, false, false, true));
            _dict.Add(Declaration.Property, new DeclarationDescription(Declaration.Property, "Property", true, false, true, true, false, true));
            _dict.Add(Declaration.Struct, new DeclarationDescription(Declaration.Struct, "Struct", true, false, true, false, false, false));
            _dict.Add(Declaration.Variable, new DeclarationDescription(Declaration.Variable, "Variable", false, false, true, false, false, true));

            _dict.Add(Declaration.Constructor, new DeclarationDescription(Declaration.Constructor, "Constructor", true, false, true, true, false, false));
            _dict.Add(Declaration.Destructor, new DeclarationDescription(Declaration.Destructor, "Destructor", true, false, true, false, false, false));
            _dict.Add(Declaration.Indexer, new DeclarationDescription(Declaration.Indexer, "Indexer", true, false, true, true, false, false));
            _dict.Add(Declaration.Operator, new DeclarationDescription(Declaration.Operator, "Operator", true, false, true, false, false, false));
        }

        public DeclarationDescription(Declaration declaration, string name, bool hasAccessLevel, bool canInherit,
                                      bool canBeMarkedWithAttribute, bool canBeStatic, bool canBeReadOnly, bool ownsType)
        {
            Declaration = declaration;
            Name = name;
            HasAccessLevel = hasAccessLevel;
            CanInherit = canInherit;
            CanBeMarkedWithAttribute = canBeMarkedWithAttribute;
            CanBeStatic = canBeStatic;
            CanBeReadonly = canBeReadOnly;
            OwnsType = ownsType;
        }

        public static Dictionary<Declaration, DeclarationDescription> DeclDescriptions
        {
            get { return _dict; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}