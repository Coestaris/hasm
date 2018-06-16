namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt48 : MemZoneVariable
    {
        public UInt48 Value;

        public MemZoneVariableUInt48(UInt48 value, string name)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt48(UInt48 value, int uid)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            UID = uid;
        }

        public MemZoneVariableUInt48(UInt48 value, int uid, string name)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            UID = uid;
            Name = name;
        }
    }
}