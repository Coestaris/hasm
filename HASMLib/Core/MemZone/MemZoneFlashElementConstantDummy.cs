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
        public void UpdateValue(UIntSingle value, int index)
        {
            isEmpty = false;
            Length = LengthQualifier.Single;
            Value = new UIntSingle[] { value };
        }

        public void UpdateValue(UIntDouble value, int index)
        {
            isEmpty = false;

            Length = LengthQualifier.Double;
            Value = value.ToUInt12();
        }

        public void UpdateValue(UIntQuad value, int index)
        {
            isEmpty = false;

            Length = LengthQualifier.Quad;
            Value = value.ToUInt12();
        }
    }
}