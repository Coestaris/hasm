using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
    public class FlashElementConstantDummy : FlashElementConstant
    {
        public bool isEmpty = true;

        public FlashElementConstantDummy(Integer index) : base(new Integer(), index) { }

        //MAKE CONSTANT NOT DUMMY AGAIN!
        public void UpdateValue(Integer value, Integer index)
        {
            isEmpty = false;
            Value = value;
        }
    }
}