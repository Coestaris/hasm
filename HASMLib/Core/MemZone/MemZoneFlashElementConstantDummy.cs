using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementConstantDummy : MemZoneFlashElementConstant
    {
        public bool isEmpty = true;

        public MemZoneFlashElementConstantDummy(Integer index) : base(new Integer(), index) { }

        //MAKE CONSTANT NOT DUMMY AGAIN!
        public void UpdateValue(Integer value, Integer index)
        {
            isEmpty = false;
            Value = value;
        }
    }
}