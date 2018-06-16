namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt24 : MemZoneVariable
    {
        public UInt24 Value;

        public MemZoneVariableUInt24(UInt24 value, string name)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt24(UInt24 value, int uid)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Index = uid;
        }
        
        public MemZoneVariableUInt24(UInt24 value, int uid, string name)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Index = uid;
            Name = name;
        }
    }
}