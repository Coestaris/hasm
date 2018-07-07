namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt48 : MemZoneVariable
    {
        public UIntQuad Value;

        public MemZoneVariableUInt48(UIntQuad value, string name)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt48(UIntQuad value, int uid)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            Index = uid;
        }

        public MemZoneVariableUInt48(UIntQuad value, int uid, string name)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            Index = uid;
            Name = name;
        }
    }
}