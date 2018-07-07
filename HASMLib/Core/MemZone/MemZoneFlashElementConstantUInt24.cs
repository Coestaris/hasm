namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantUInt24 : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantUInt24(UIntDouble value, int index)
        {
			Length = LengthQualifier.Double;
            Value = value.ToUInt12();
			Index = index;
		}
    }
}