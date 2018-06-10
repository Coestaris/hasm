namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantUInt48 : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantUInt48(UInt48 value, int index)
        {
			Length = Length_Quad;
            Value = value.ToUInt12();
			Index = index;
		}
    }
}