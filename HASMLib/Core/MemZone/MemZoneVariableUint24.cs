namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt24 : MemZoneVariable
    {
        public UInt24 Value;

        public MemZoneVariableUInt24(UInt24 value, string name)
        {
            Length = MemZoneVariableLength.Double;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt24(UInt24 value, int uid)
        {
            Length = MemZoneVariableLength.Double;
            Value = value;
            UID = uid;
        }
    }
}