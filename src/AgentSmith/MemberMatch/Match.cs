using System.Collections.Generic;
using System.Text;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentSmith.MemberMatch
{
    public class Match
    {
        private static readonly Dictionary<DeclaredElementType, Declaration> _declMap =
            new Dictionary<DeclaredElementType, Declaration>();

        private AccessLevels _accessLevel = AccessLevels.Any;
        private Declaration _declaration = Declaration.Any;
        private string _inheritedFrom;
        private string _isOfType;
        private string _markedWithAttribute;
        private FuzzyBool _readOnly = FuzzyBool.Maybe;
        private FuzzyBool _static = FuzzyBool.Maybe;

        private ITypeElement _markedWithAttributeType;
        private ITypeElement _inheritedFromType;
        private ITypeElement _isOfTypeType;
        private ParamDirection _paramDirection = ParamDirection.Any;
        private static readonly Dictionary<RightsPair, AccessRights> _rightsMap;

        static Match()
        {
            _declMap.Add(CLRDeclaredElementType.CLASS, Declaration.Class);
            _declMap.Add(CLRDeclaredElementType.CONSTANT, Declaration.Constant);
            _declMap.Add(CLRDeclaredElementType.DELEGATE, Declaration.Delegate);
            _declMap.Add(CLRDeclaredElementType.ENUM, Declaration.Enum);
            _declMap.Add(CLRDeclaredElementType.ENUM_MEMBER, Declaration.EnumerationMember);
            _declMap.Add(CLRDeclaredElementType.EVENT, Declaration.Event);
            _declMap.Add(CLRDeclaredElementType.FIELD, Declaration.Field);
            _declMap.Add(CLRDeclaredElementType.INTERFACE, Declaration.Interface);
            //_declMap.Add(CSharpDeclaredElementType. LOCAL_CONSTANT, Declaration.Constant);
            _declMap.Add(CLRDeclaredElementType.LOCAL_VARIABLE, Declaration.Variable);
            _declMap.Add(CLRDeclaredElementType.METHOD, Declaration.Method);
            _declMap.Add(CLRDeclaredElementType.NAMESPACE, Declaration.Namespace);
            _declMap.Add(CLRDeclaredElementType.PARAMETER, Declaration.Parameter);
            _declMap.Add(CLRDeclaredElementType.PROPERTY, Declaration.Property);
            _declMap.Add(CLRDeclaredElementType.STRUCT, Declaration.Struct);

            _rightsMap = createRightsMap();
        }

        private struct RightsPair
        {
            public AccessRights Child;
            public AccessRights Parent;            

            public RightsPair(AccessRights child, AccessRights parent)
            {
                Child = child;
                Parent = parent;                
            }
        }

        private static Dictionary<RightsPair, AccessRights> createRightsMap()
        {
            Dictionary<RightsPair, AccessRights> map = new Dictionary<RightsPair, AccessRights>();
            map.Add(new RightsPair(AccessRights.PUBLIC, AccessRights.PUBLIC), AccessRights.PUBLIC);
            map.Add(new RightsPair(AccessRights.PUBLIC, AccessRights.INTERNAL), AccessRights.INTERNAL);
            map.Add(new RightsPair(AccessRights.PUBLIC, AccessRights.PRIVATE), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PUBLIC, AccessRights.PROTECTED), AccessRights.PROTECTED);
            map.Add(new RightsPair(AccessRights.PUBLIC, AccessRights.PROTECTED_AND_INTERNAL), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.PUBLIC, AccessRights.PROTECTED_OR_INTERNAL), AccessRights.PROTECTED_OR_INTERNAL);

            map.Add(new RightsPair(AccessRights.INTERNAL, AccessRights.PUBLIC), AccessRights.INTERNAL);
            map.Add(new RightsPair(AccessRights.INTERNAL, AccessRights.INTERNAL), AccessRights.INTERNAL);
            map.Add(new RightsPair(AccessRights.INTERNAL, AccessRights.PRIVATE), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.INTERNAL, AccessRights.PROTECTED), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.INTERNAL, AccessRights.PROTECTED_AND_INTERNAL), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.INTERNAL, AccessRights.PROTECTED_OR_INTERNAL), AccessRights.INTERNAL);

            map.Add(new RightsPair(AccessRights.PRIVATE, AccessRights.PUBLIC), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PRIVATE, AccessRights.INTERNAL), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PRIVATE, AccessRights.PRIVATE), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PRIVATE, AccessRights.PROTECTED), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PRIVATE, AccessRights.PROTECTED_AND_INTERNAL), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PRIVATE, AccessRights.PROTECTED_OR_INTERNAL), AccessRights.PRIVATE);

            map.Add(new RightsPair(AccessRights.PROTECTED, AccessRights.PUBLIC), AccessRights.PROTECTED);
            map.Add(new RightsPair(AccessRights.PROTECTED, AccessRights.INTERNAL), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED, AccessRights.PRIVATE), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PROTECTED, AccessRights.PROTECTED), AccessRights.PROTECTED);
            map.Add(new RightsPair(AccessRights.PROTECTED, AccessRights.PROTECTED_AND_INTERNAL), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED, AccessRights.PROTECTED_OR_INTERNAL), AccessRights.PROTECTED);

            map.Add(new RightsPair(AccessRights.PROTECTED_AND_INTERNAL, AccessRights.PUBLIC), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED_AND_INTERNAL, AccessRights.INTERNAL), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED_AND_INTERNAL, AccessRights.PRIVATE), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PROTECTED_AND_INTERNAL, AccessRights.PROTECTED), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED_AND_INTERNAL, AccessRights.PROTECTED_AND_INTERNAL), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED_AND_INTERNAL, AccessRights.PROTECTED_OR_INTERNAL), AccessRights.PROTECTED_AND_INTERNAL);

            map.Add(new RightsPair(AccessRights.PROTECTED_OR_INTERNAL, AccessRights.PUBLIC), AccessRights.PROTECTED_OR_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED_OR_INTERNAL, AccessRights.INTERNAL), AccessRights.PROTECTED_OR_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED_OR_INTERNAL, AccessRights.PRIVATE), AccessRights.PRIVATE);
            map.Add(new RightsPair(AccessRights.PROTECTED_OR_INTERNAL, AccessRights.PROTECTED), AccessRights.PROTECTED);
            map.Add(new RightsPair(AccessRights.PROTECTED_OR_INTERNAL, AccessRights.PROTECTED_AND_INTERNAL), AccessRights.PROTECTED_AND_INTERNAL);
            map.Add(new RightsPair(AccessRights.PROTECTED_OR_INTERNAL, AccessRights.PROTECTED_OR_INTERNAL), AccessRights.PROTECTED_OR_INTERNAL);

            return map;
        }

        public Match()
        {
        }

        public Match(Declaration declaration)
        {
            _declaration = declaration;
        }

        public Match(Declaration declaration, AccessLevels accessLevel)
        {
            _accessLevel = accessLevel;
            _declaration = declaration;
        }

        public Match(Declaration declaration, AccessLevels accessLevel, string inheritedFrom, string isOfType)
        {
            _accessLevel = accessLevel;
            _declaration = declaration;
            _inheritedFrom = inheritedFrom;
            _isOfType = isOfType;
        }

        public Match(Declaration declaration, AccessLevels accessLevel, string inheritedFrom, string isOfType, string markedWithAttribute)
        {
            _accessLevel = accessLevel;
            _declaration = declaration;
            _inheritedFrom = inheritedFrom;
            _markedWithAttribute = markedWithAttribute;
            _isOfType = isOfType;
        }

        public AccessLevels AccessLevel
        {
            get { return _accessLevel; }
            set { _accessLevel = value; }
        }

        public Declaration Declaration
        {
            get { return _declaration; }
            set { _declaration = value; }
        }

        public string InheritedFrom
        {
            get { return _inheritedFrom; }
            set { _inheritedFrom = value; }
        }

        public string MarkedWithAttribute
        {
            get { return _markedWithAttribute; }
            set { _markedWithAttribute = value; }
        }

        public FuzzyBool IsReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        public FuzzyBool IsStatic
        {
            get { return _static; }
            set { _static = value; }
        }

        public string IsOfType
        {
            get { return _isOfType; }
            set { _isOfType = value; }
        }

        public ParamDirection ParamDirection
        {
            get { return _paramDirection; }
            set { _paramDirection = value; }
        }

		//public void Prepare(ISolution solution, PsiManager manager)
		//{
		//	_markedWithAttributeType = null;
		//	_inheritedFromType = null;
		//	_isOfTypeType = null;
			
		//	IPsiModule psiModule = new EmptyPsiModule(solution);
		//	ISymbolScope symbolScope = solution.GetPsiServices()
		//							 .Symbols.GetSymbolScope(LibrarySymbolScope.FULL, true, psiModule.GetContextFromModule());
		//	if (!string.IsNullOrEmpty(_markedWithAttribute))
		//	{
                
		//		_markedWithAttributeType = symbolScope.GetTypeElementByCLRName(_markedWithAttribute);
		//	}

		//	if (!string.IsNullOrEmpty(_inheritedFrom))
		//	{
		//		_inheritedFromType = symbolScope.GetTypeElementByCLRName(_inheritedFrom);
		//	}

		//	if (!string.IsNullOrEmpty(_isOfType))
		//	{
		//		_isOfTypeType = symbolScope.GetTypeElementByCLRName(_isOfType);
		//	}
		//}

        public bool IsMatch(IDeclaration declaration, bool useEffectiveRights)
        {
            if (declaration == null)
            {
                return false;
            }
            if (declaration is INamespaceDeclaration)
            {
                return _declaration == Declaration.Any || _declaration == Declaration.Namespace;
            }
            else
            {
                return isDeclMatch(declaration) &&
                       isRightsMatch(declaration, useEffectiveRights) &&
                       markedWithAttributeMatch(declaration) &&
                       inheritsMatch(declaration) &&
                       ownsTypeMatch(declaration) &&
                       isReadOnlyMatch(declaration) &&
                       isStaticMatch(declaration) && 
                       paramDirectionMatch(declaration);
            }
        }

        private bool paramDirectionMatch(IDeclaration declaration)
        {
            if (_paramDirection == ParamDirection.Any)
            {
                return true;
            }

            IParameter param = declaration.DeclaredElement as IParameter;
            return param != null && (param.Kind == ParameterKind.OUTPUT && (_paramDirection & ParamDirection.Out) != 0 ||
                                     param.Kind == ParameterKind.REFERENCE && (_paramDirection & ParamDirection.Ref) != 0 ||
                                     param.Kind == ParameterKind.VALUE && (_paramDirection & ParamDirection.In) != 0);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            DeclarationDescription description = DeclarationDescription.DeclDescriptions[_declaration];
            if (description.HasAccessLevel)
            {
                sb.AppendFormat("{0} ", getAccessLevel());
            }
            if (_static != FuzzyBool.Maybe)
            {
                sb.Append(_static == FuzzyBool.True ? "static " : "not static ");
            }
            if (_readOnly != FuzzyBool.Maybe)
            {
                sb.Append(_readOnly == FuzzyBool.True ? "readonly " : "not readonly ");
            }
            if (Declaration == Declaration.Parameter && ParamDirection != ParamDirection.Any)
            {
                sb.AppendFormat("{0} ", ParamDirection);
            }
            sb.AppendFormat("{0} ", description.Declaration == Declaration.Any ? "declaration" : description.Name.ToLower());
            if (description.CanInherit && !string.IsNullOrEmpty(_inheritedFrom))
            {
                sb.AppendFormat("inherited from '{0}' ", _inheritedFrom);
            }
            if (description.OwnsType && !string.IsNullOrEmpty(_isOfType))
            {
                sb.AppendFormat("with type '{0}' ", _isOfType);
            }
            if (description.CanBeMarkedWithAttribute && !string.IsNullOrEmpty(_markedWithAttribute))
            {
                sb.AppendFormat("marked with '{0}' ", _markedWithAttribute);
            }
            return sb.ToString();
        }

        private bool isReadOnlyMatch(IDeclaration declaration)
        {            
            if (IsReadOnly == FuzzyBool.Maybe)
            {
                return true;
            }
            IClassMemberDeclaration decl = declaration as IClassMemberDeclaration;
            return decl != null && (IsReadOnly == FuzzyBool.True && decl.IsReadonly ||
                                    IsReadOnly == FuzzyBool.False && !decl.IsReadonly);
        }

        private bool isStaticMatch(IDeclaration declaration)
        {
            if (IsStatic == FuzzyBool.Maybe)
            {
                return true;
            }
            IClassMemberDeclaration decl = declaration as IClassMemberDeclaration;
            return decl != null && (IsStatic == FuzzyBool.True && decl.IsStatic ||
                                    IsStatic == FuzzyBool.False && !decl.IsStatic);
        }

        private bool ownsTypeMatch(IDeclaration declaration)
        {
            if (string.IsNullOrEmpty(_isOfType))
            {
                return true;
            }

            if (_isOfTypeType == null)
            {                
                return false;
            }
            
            if (declaration is ITypeOwner)
            {                
                IDeclaredType declaredType = ((ITypeOwner)declaration).Type as IDeclaredType;
                if (declaredType == null)
                {
                    return false;
                }
                ITypeElement typeElement = declaredType.GetTypeElement();
                if (typeElement == null)
                {
                    return false;
                }                
                return typeElement.IsDescendantOf(_isOfTypeType);
            } 
            else
            {                
                return false;
            }      
        }

        private bool inheritsMatch(IDeclaration declaration)
        {
            if (string.IsNullOrEmpty(_inheritedFrom))
            {
                return true;
            }
            
            if (_inheritedFromType == null)
            {
                return false;
            }

            ITypeElement typeElement = declaration.DeclaredElement as ITypeElement;
            if (typeElement == null)
            {
                return false;                
            }
            return typeElement.IsDescendantOf(_inheritedFromType);
        }

        private bool markedWithAttributeMatch(IDeclaration declaration)
        {
            if (string.IsNullOrEmpty(_markedWithAttribute))
            {
                return true;
            }

            if (_markedWithAttributeType == null)
            {
                return false;
            }

            IAttributesOwner attributesOwner = declaration.DeclaredElement as IAttributesOwner;
            if (attributesOwner == null)
            {
                return false;
            }

            foreach (IAttributeInstance attribute in attributesOwner.GetAttributeInstances(false))
            {
                if (Equals(attribute.GetAttributeType().GetTypeElement(), _markedWithAttributeType))
                {
                    return true;
                }
            }
            return false;
        }

        private bool isRightsMatch(IDeclaration declaration, bool useEffectiveRights)
        {
            if (_accessLevel == AccessLevels.Any)
            {
                return true;
            }

            AccessRights rights;
            if (declaration is IEnumMemberDeclaration)
            {
                declaration = ((IEnumMemberDeclaration) declaration).GetContainingTypeDeclaration();
            }

            if (declaration is IParameterDeclaration)
            {
                declaration = declaration.GetContainingNode<ITypeMemberDeclaration>();
            }
            
            if (!(declaration is IModifiersOwner))
            {
                return false;
            }
            rights = getRights((IModifiersOwner)declaration, useEffectiveRights);
            
            return AccessLevelMap.Map.ContainsKey(rights) && ((AccessLevelMap.Map[rights] & _accessLevel) != 0);
        }

        private static AccessRights getRights(IModifiersOwner owner, bool useEffectiveRights)
        {
            AccessRights effectiveRights = owner.GetAccessRights();
            if (!useEffectiveRights || !(owner is IClassMemberDeclaration))
            {
                return effectiveRights;
            }

            owner = ((IClassMemberDeclaration)owner).GetContainingTypeDeclaration();
            while (owner != null)
            {
                AccessRights ownerRights = owner.GetAccessRights();
                AccessRights newEffectiveRights;
                if (_rightsMap.TryGetValue(new RightsPair(effectiveRights, ownerRights), out newEffectiveRights))
                {
                    effectiveRights = newEffectiveRights;
                }
                
                owner = ((IClassMemberDeclaration)owner).GetContainingTypeDeclaration();
            }

            return effectiveRights;
        }

        private bool isDeclMatch(IDeclaration declaration)
        {
            if (declaration is IAccessorDeclaration)
            {
                return false;
            }

            if (declaration.DeclaredElement != null)
            {
                DeclaredElementType type = declaration.DeclaredElement.GetElementType();
                return _declaration == Declaration.Any ||
                       _declMap.ContainsKey(type) && _declMap[type] == _declaration;
            }
            else
            {
                return false;
            }
        }

        private string getAccessLevel()
        {
            StringBuilder sb = new StringBuilder();

            if (AccessLevel == AccessLevels.Any)
            {
                sb.Append("Any");
            }
            else
            {
                foreach (AccessLevelDescription desc in AccessLevelDescription.Descriptions.Values)
                {
                    if (desc.AccessLevel != AccessLevels.Any && (AccessLevel & desc.AccessLevel) != 0)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(", ");
                        }
                        sb.Append(desc.Name);
                    }
                }
            }
            return sb.ToString();
        }
    }
}