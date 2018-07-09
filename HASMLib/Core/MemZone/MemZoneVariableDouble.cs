using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneVariableDouble : MemZoneVariable
    {
        public FDouble Value { get; internal set; }

        public MemZoneVariableDouble(FDouble value, string name)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Name = name;
        }

        public MemZoneVariableDouble(FDouble value, int uid)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Index = uid;
        }
        
        public MemZoneVariableDouble(FDouble value, int uid, string name)
        {
            Length = LengthQualifier.Double;
            Value = value;
            Index = uid;
            Name = name;
        }
    }
}