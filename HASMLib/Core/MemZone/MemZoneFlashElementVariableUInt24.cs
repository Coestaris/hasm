namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementVariableUInt24 : MemZoneFlashElementVariable
    {
        public MemZoneFlashElementVariableUInt24(UInt24 value, string name)
        {
            Length = 2;
            Value = value.ToUInt12();
            Name = name;
        }
    }
}