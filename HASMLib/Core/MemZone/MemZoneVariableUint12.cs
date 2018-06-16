namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt12 : MemZoneVariable
    {
        public UInt12 Value;

        public MemZoneVariableUInt12(UInt12 value, string name)
        {
            Length = MemZoneVariableLength.Single;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt12(UInt12 value, int uid)
        {
            Length = MemZoneVariableLength.Single;
            Value = value;
            UID = uid;
        }
    }
}