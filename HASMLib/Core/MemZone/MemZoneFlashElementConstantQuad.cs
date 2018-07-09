using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantQuad : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantQuad(FQuad value, int index)
        {
			Length = LengthQualifier.Quad;
            Value = value.ToSingle();
			Index = index;
		}
    }
}