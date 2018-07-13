using HASMLib.Core.BaseTypes;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneVariable
    {
        public string Name { get; protected set; }
        public Integer Index { get; protected set; }
        public Integer Value { get; set; }

        public MemZoneVariable(BaseIntegerType type, Integer index) : this(new Integer(0, type), index) 
        { }

        public MemZoneVariable(Integer value, Integer index)
        {
            Value = value;
            Index = index;
        }

        public void AddValue(MemZoneVariable value)
        {
            Value += value.Value;
        }

        public void SetValue(MemZoneVariable value)
        {
            Value = value.Value;
        }

        public void SetValue(ulong value)
        {
            Value = new Integer(value, Value.Type);
        }

        public void AddValue(ulong value)
        {
            Value = new Integer(Value.Value + value, Value.Type);
        }
    }
}