using HASMLib.Core.BaseTypes;
using HASMLib.Parser.SyntaxTokens.Constants;

namespace HASMLib.Core.MemoryZone
{
    public class FlashElementConstantDummy : FlashElementConstant
    {
        public bool isEmpty = true;

        public FlashElementConstantDummy(Integer index) : base(new Constant(BaseIntegerType.CommonType), index) { }

        //MAKE CONSTANT NOT DUMMY AGAIN!
        public void UpdateValue(Constant value, Integer index)
        {
            isEmpty = false;
            Value = value;
        }
    }
}