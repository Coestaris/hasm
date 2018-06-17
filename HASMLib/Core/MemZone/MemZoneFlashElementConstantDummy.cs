namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantDummy : MemZoneFlashElementConstant
    {
        public bool isEmpty = true;

		public MemZoneFlashElementConstantDummy(int index)
        {
			Index = index;
		}

        //MAKE CONSTANT NOT DUMMY AGAIN!
        public void UpdateValue(UInt12 value, int index)
        {
            isEmpty = false;
            Length = LengthQualifier.Single;
            Value = new UInt12[] { value };
        }

        public void UpdateValue(UInt24 value, int index)
        {
            isEmpty = false;

            Length = LengthQualifier.Double;
            Value = value.ToUInt12();
        }

        public void UpdateValue(UInt48 value, int index)
        {
            isEmpty = false;

            Length = LengthQualifier.Quad;
            Value = value.ToUInt12();
        }
    }
}