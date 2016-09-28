using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace AgentSmith.SpellCheck
{
    public class TokenNodeTypeBase: TokenNodeType
    {
        public TokenNodeTypeBase(string name, int index) : base(name, index)
        {
        }

        public override bool IsWhitespace
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsComment
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsStringLiteral
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsConstantLiteral { get { throw new NotImplementedException(); } }

        public override bool IsIdentifier { get { throw new NotImplementedException(); } }

        public override bool IsKeyword { get { throw new NotImplementedException(); } }

	    public override string TokenRepresentation {
		    get { throw new NotImplementedException(); }
	    }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            throw new NotImplementedException();
        }
    }
}