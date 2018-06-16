namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt12 : MemZoneVariable
    {
        public UInt12 Value;

        public MemZoneVariableUInt12(UInt12 value, string name)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt12(UInt12 value, int uid)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Index = uid;
        }

        public MemZoneVariableUInt12(UInt12 value, int uid, string name)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Index = uid;
            Name = name;
        }
    }
}