using HASMLib.Core.BaseTypes;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneVariable
    {
        public LengthQualifier Length { get; protected set; }
        public string Name { get; protected set; }
        public int Index { get; protected set; }

        public void AddValue(MemZoneVariable value)
        {
            AddValue(value.GetNumericValue());
        }

        public void SetValue(MemZoneVariable value)
        {
            SetValue(value.GetNumericValue());
        }

        public List<FSingle> ToSingle()
        {
            switch(Length)
            {
                case LengthQualifier.Single:
                    return new List<FSingle> { (this as MemZoneVariableSingle).Value };
                case LengthQualifier.Double:
                    return (this as MemZoneVariableDouble).Value.ToSingle().ToList();
                case LengthQualifier.Quad:
                    return (this as MemZoneVariableQuad).Value.ToSingle().ToList();
                default:
                    return null;
            }
        }

        public void SetValue(long value)
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    (this as MemZoneVariableSingle).Value = (FSingle)value;
                    break;
                case LengthQualifier.Double:
                    (this as MemZoneVariableDouble).Value = (FDouble)value;
                    break;
                case LengthQualifier.Quad:
                    (this as MemZoneVariableQuad).Value = (FQuad)value;
                    break;
            }
        }

        public void AddValue(long value)
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    (this as MemZoneVariableSingle).Value += (FSingle)value;
                    break;
                case LengthQualifier.Double:
                    (this as MemZoneVariableDouble).Value += (FDouble)value;
                    break;
                case LengthQualifier.Quad:
                    (this as MemZoneVariableQuad).Value += (FQuad)value;
                    break;
            }
        }

        public long GetNumericValue()
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    return (this as MemZoneVariableSingle).Value;
                case LengthQualifier.Double:
                    return (this as MemZoneVariableDouble).Value;
                case LengthQualifier.Quad:
                    return (this as MemZoneVariableQuad).Value;

                default:
                    return 0;
            }
        }
    }
}