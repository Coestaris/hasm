using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
	public class MemZoneFlashElementConstantSingle : MemZoneFlashElementConstant
    {
		public MemZoneFlashElementConstantSingle(FSingle value, int index)
        {
			Length = LengthQualifier.Single;
			Value = new FSingle[]{ value };
			Index = index;
		}
    }
}