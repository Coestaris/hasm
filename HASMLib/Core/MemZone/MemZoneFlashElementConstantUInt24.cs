namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantUInt24 : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantUInt24(UInt24 value, int index)
        {
			Length = LengthQualifier.Double;
            Value = value.ToUInt12();
			Index = index;
		}
    }
}