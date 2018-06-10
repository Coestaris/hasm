namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElement
    {
        public virtual int FixedSize => 0;
        public virtual MemZoneFlashElementType Type => MemZoneFlashElementType.Undefined;
       
    }
}