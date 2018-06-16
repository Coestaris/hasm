
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Core;

namespace HASMLib.Parser
{
    internal partial class HASMParser
	{
        internal struct NamedConstant
        {
            public string Name;
            public UInt24 Index;
            public Constant Value;

            public NamedConstant(string name, UInt24 index, Constant value)
            {
                Name = name;
                Index = index;
                Value = value;
            }
        }
	}
}


