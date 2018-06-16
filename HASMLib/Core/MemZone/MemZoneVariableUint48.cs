namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt48 : MemZoneVariable
    {
        public UInt48 Value;

        public MemZoneVariableUInt48(UInt48 value, string name)
        {
            Length = MemZoneVariableLength.Quad;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt48(UInt48 value, int uid)
        {
            Length = MemZoneVariableLength.Quad;
            Value = value;
            UID = uid;
        }
    }
}