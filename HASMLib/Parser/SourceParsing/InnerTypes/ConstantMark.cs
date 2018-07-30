using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.Constants;

namespace HASMLib.Parser
{
    public class ConstantMark
    {
        public FlashElementConstant FEReference { get; internal set; }
        public string Name { get; private set; }
        public Integer Index { get; private set; }
        public Constant Constant { get; internal set; }

        public ConstantMark(string name, Integer index, Constant value)
        {
            Name = name;
            Index = index;
            Constant = value;
        }
    }
}


