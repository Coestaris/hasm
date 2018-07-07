namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt12 : MemZoneVariable
    {
        public UIntSingle Value;

        public MemZoneVariableUInt12(UIntSingle value, string name)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt12(UIntSingle value, int uid)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Index = uid;
        }

        public MemZoneVariableUInt12(UIntSingle value, int uid, string name)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Index = uid;
            Name = name;
        }
    }
}