namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantUInt12 : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantUInt12(UInt12 value, int index)
        {
			Length = Length_Single;
			Value = new UInt12[]{ value };
			Index = index;
		}
    }
}