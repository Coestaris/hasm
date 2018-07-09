using HASMLib.Core.BaseTypes;

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
        public void UpdateValue(FSingle value, int index)
        {
            isEmpty = false;
            Length = LengthQualifier.Single;
            Value = new FSingle[] { value };
        }

        public void UpdateValue(FDouble value, int index)
        {
            isEmpty = false;

            Length = LengthQualifier.Double;
            Value = value.ToSingle();
        }

        public void UpdateValue(FQuad value, int index)
        {
            isEmpty = false;

            Length = LengthQualifier.Quad;
            Value = value.ToSingle();
        }
    }
}