using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableQuad : MemZoneVariable
    {
        public FQuad Value { get; internal set; }

        public MemZoneVariableQuad(FQuad value, string name)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            Name = name;
        }

        public MemZoneVariableQuad(FQuad value, int uid)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            Index = uid;
        }

        public MemZoneVariableQuad(FQuad value, int uid, string name)
        {
            Length = LengthQualifier.Quad;
            Value = value;
            Index = uid;
            Name = name;
        }
    }
}