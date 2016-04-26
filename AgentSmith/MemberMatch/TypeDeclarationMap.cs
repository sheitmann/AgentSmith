using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentSmith.MemberMatch
{
    public static class TypeDeclarationMap
    {
        private static readonly Dictionary<Type, Declaration> _typeDeclMap =
            new Dictionary<Type, Declaration>();

        static TypeDeclarationMap()
        {
            _typeDeclMap.Add(typeof(IConstantDeclaration), Declaration.Constant);
            _typeDeclMap.Add(typeof(IEnumDeclaration), Declaration.Enum);
            _typeDeclMap.Add(typeof(IInterfaceDeclaration), Declaration.Interface);
            _typeDeclMap.Add(typeof(IStructDeclaration), Declaration.Struct);
            _typeDeclMap.Add(typeof(IClassDeclaration), Declaration.Class);
            _typeDeclMap.Add(typeof(IDelegateDeclaration), Declaration.Delegate);
            _typeDeclMap.Add(typeof(IFieldDeclaration), Declaration.Field);
            _typeDeclMap.Add(typeof(IConstructorDeclaration), Declaration.Constructor);
            _typeDeclMap.Add(typeof(IDestructorDeclaration), Declaration.Destructor);
            _typeDeclMap.Add(typeof(IEventDeclaration), Declaration.Event);
            _typeDeclMap.Add(typeof(IPropertyDeclaration), Declaration.Property);
            _typeDeclMap.Add(typeof(IIndexerDeclaration), Declaration.Indexer);
            _typeDeclMap.Add(typeof(IOperatorDeclaration), Declaration.Operator);
            _typeDeclMap.Add(typeof(IMethodDeclaration), Declaration.Method);
        }

        public static Dictionary<Type, Declaration> Map
        {
            get { return _typeDeclMap; }
        }
    }
}