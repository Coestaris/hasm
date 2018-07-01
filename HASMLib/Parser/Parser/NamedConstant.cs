
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Core;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class NamedConstant
    {
        public MemZoneFlashElementConstant constant;

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


