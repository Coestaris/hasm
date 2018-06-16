using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens;

namespace HASMLib.Parser
{
    public partial class HASMParser
	{
        public struct Variable
        {
            public string Name;
            public LengthQualifier Length;

            public Variable(string name, LengthQualifier length)
            {
                Name = name;
                Length = length;
            }
        }
	}
}


