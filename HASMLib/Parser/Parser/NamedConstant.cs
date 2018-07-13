using HASMLib.Core.BaseTypes;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class NamedConstant
    {
        public MemZoneFlashElementConstant FEReference { get; internal set; }
        public string Name { get; private set; }
        public Integer Index { get; private set; }
        public Constant Constant { get; internal set; }

        public NamedConstant(string name, Integer index, Constant value)
        {
            Name = name;
            Index = index;
            Constant = value;
        }
    }
}


