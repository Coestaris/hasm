namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantUInt12 : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantUInt12(UIntSingle value, int index)
        {
			Length = LengthQualifier.Single;
			Value = new UIntSingle[]{ value };
			Index = index;
		}
    }
}