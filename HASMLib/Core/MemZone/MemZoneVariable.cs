using HASMLib.Parser.SyntaxTokens;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneVariable
    {
        public void AddValue(MemZoneVariable value)
        {
            AddValue(value.GetNumericValue());
        }

        public void SetValue(MemZoneVariable value)
        {
            SetValue(value.GetNumericValue());
        }

        public List<UInt12> ToUInt12()
        {
            switch(Length)
            {
                case LengthQualifier.Single:
                    return new List<UInt12> { (this as MemZoneVariableUInt12).Value };
                case LengthQualifier.Double:
                    return (this as MemZoneVariableUInt24).Value.ToUInt12().ToList();
                case LengthQualifier.Quad:
                    return (this as MemZoneVariableUInt48).Value.ToUInt12().ToList();
                default:
                    return null;
            }
        }

        public void SetValue(long value)
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    (this as MemZoneVariableUInt12).Value = (UInt12)value;
                    break;
                case LengthQualifier.Double:
                    (this as MemZoneVariableUInt24).Value = (UInt24)value;
                    break;
                case LengthQualifier.Quad:
                    (this as MemZoneVariableUInt48).Value = (UInt48)value;
                    break;
            }
        }

        public void AddValue(long value)
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    (this as MemZoneVariableUInt12).Value += (UInt12)value;
                    break;
                case LengthQualifier.Double:
                    (this as MemZoneVariableUInt24).Value += (UInt24)value;
                    break;
                case LengthQualifier.Quad:
                    (this as MemZoneVariableUInt48).Value += (UInt48)value;
                    break;
            }
        }

        public long GetNumericValue()
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    return (this as MemZoneVariableUInt12).Value;
                case LengthQualifier.Double:
                    return (this as MemZoneVariableUInt24).Value;
                case LengthQualifier.Quad:
                    return (this as MemZoneVariableUInt48).Value;

                default:
                    return 0;
            }
        }

        public LengthQualifier Length;

        public string Name;
        public int Index;
    }
}