namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementVariable : MemZoneFlashElement
    {
        public string Name;

        protected int Length;
        protected UInt12[] Value;

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Variable;
        public override int FixedSize => Length;
    }
}