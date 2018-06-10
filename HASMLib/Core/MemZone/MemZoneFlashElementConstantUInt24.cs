namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantUInt24 : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantUInt24(UInt24 value, int index)
        {
			Length = Length_Double;
            Value = value.ToUInt12();
			Index = index;
		}
    }
}