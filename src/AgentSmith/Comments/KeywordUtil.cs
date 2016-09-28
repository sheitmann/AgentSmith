using System;
using System.Collections.Generic;

namespace AgentSmith.Comments
{
    public static class KeywordUtil
    {
        private static readonly HashSet<string> _keywords = new HashSet<string>
                                                                {
                                                                    "null",
                                                                    "sealed",
                                                                    "static",
                                                                    "abstract",
                                                                    "virtual",
                                                                    "true",
                                                                    "false",
                                                                    //following are keywords not 
                                                                    //mentioned in NDoc documentation

                                                                    //"event", 
                                                                    //"new", 
                                                                    "struct",
                                                                    //"as",
                                                                    //"explicit",                                                                  
                                                                    "switch",
                                                                    "base",
                                                                    "extern",
                                                                    "object",
                                                                    "this",
                                                                    "bool",
                                                                    "operator",
                                                                    "throw",
                                                                    "break",
                                                                    "finally",
                                                                    //"out",                                                                    
                                                                    "byte",
                                                                    "fixed",
                                                                    "override",
                                                                    //"try", 
                                                                    //"case",
                                                                    "float",
                                                                    "params",
                                                                    "typeof",
                                                                    "catch",
                                                                    //"for",
                                                                    "private",
                                                                    "uint",
                                                                    "char",
                                                                    "foreach",
                                                                    "protected",
                                                                    "ulong",
                                                                    "checked",
                                                                    "goto",
                                                                    "public",
                                                                    "unchecked",
                                                                    //"class",
                                                                    //"if",
                                                                    //"readonly",
                                                                    //"unsafe", 
                                                                    "const",
                                                                    "implicit",
                                                                    "ref",
                                                                    "ushort",
                                                                    //"continue",
                                                                    //"in",
                                                                    //"return",
                                                                    //"using", 
                                                                    "decimal",
                                                                    "int",
                                                                    "sbyte",
                                                                    //"default",
                                                                    "interface",
                                                                    "volatile",
                                                                    "delegate",
                                                                    "internal",
                                                                    //"short",
                                                                    "void",
                                                                    //"do",
                                                                    //"is",
                                                                    "sizeof",
                                                                    "while",
                                                                    "double",
                                                                    "lock",
                                                                    "stackalloc",
                                                                    //"else",
                                                                    "long",
                                                                    "enum",
                                                                    "namespace",
                                                                    //"string"   
                                                                };
        public static bool IsKeyword(string keyword)
        {
            return _keywords.Contains(keyword);
        }
    }
}