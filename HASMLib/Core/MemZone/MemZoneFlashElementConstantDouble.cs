using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantDouble : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantDouble(FDouble value, int index)
        {
			Length = LengthQualifier.Double;
            Value = value.ToSingle();
			Index = index;
		}
    }
}