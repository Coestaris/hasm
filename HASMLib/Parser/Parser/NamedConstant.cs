
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Core;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class NamedConstant
    {
        public MemZoneFlashElementConstant constant;

        public string Name;
        public UIntDouble Index;
        public Constant Constant;

        public NamedConstant(string name, UIntDouble index, Constant value)
        {
            Name = name;
            Index = index;
            Constant = value;
        }
    }
}


