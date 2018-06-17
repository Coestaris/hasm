
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Core;

namespace HASMLib.Parser
{
    public partial class HASMParser
	{
        public class NamedConstant
        {
            public string Name;
            public UInt24 Index;
            public Constant Constant;

            public NamedConstant(string name, UInt24 index, Constant value)
            {
                Name = name;
                Index = index;
                Constant = value;
            }
        }
	}
}


