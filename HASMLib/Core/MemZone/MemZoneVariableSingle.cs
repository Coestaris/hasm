using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableSingle : MemZoneVariable
    {
        public FSingle Value { get; internal set; }

        public MemZoneVariableSingle(FSingle value, string name)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Name = name;
        }

        public MemZoneVariableSingle(FSingle value, int uid)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Index = uid;
        }

        public MemZoneVariableSingle(FSingle value, int uid, string name)
        {
            Length = LengthQualifier.Single;
            Value = value;
            Index = uid;
            Name = name;
        }
    }
}