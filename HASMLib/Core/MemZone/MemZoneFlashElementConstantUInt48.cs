namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantUInt48 : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantUInt48(UIntQuad value, int index)
        {
			Length = LengthQualifier.Quad;
            Value = value.ToUInt12();
			Index = index;
		}
    }
}