
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Core;

namespace HASMLib.Parser
{
    public partial class HASMParser
	{
        public struct NamedConstant
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


