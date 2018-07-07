namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableUInt24 : MemZoneVariable
    {
        public UIntDouble Value;

        public MemZoneVariableUInt24(UIntDouble value, string name)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Name = name;
        }

        public MemZoneVariableUInt24(UIntDouble value, int uid)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Index = uid;
        }
        
        public MemZoneVariableUInt24(UIntDouble value, int uid, string name)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Index = uid;
            Name = name;
        }
    }
}