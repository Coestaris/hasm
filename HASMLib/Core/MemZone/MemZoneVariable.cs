using HASMLib.Core.BaseTypes;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneVariable
    {
        public string Name { get; protected set; }
        public int Index { get; protected set; }
        public Integer Value { get; set; }

        public MemZoneVariable(BaseIntegerType type, int index) : this(new Integer(0, type), index) 
        { }

        public MemZoneVariable(Integer value, int index)
        {
            Value = value;
            Index = index;
        }

        public void AddValue(MemZoneVariable value)
        {
            AddValue(value.Value);
        }

        public void SetValue(MemZoneVariable value)
        {
            SetValue(value.Value);
        }

        public void SetValue(long value)
        {
            Value = new Integer(value, Value.Type);
        }

        public void AddValue(long value)
        {
            Value = new Integer(Value.Value + value, Value.Type);
        }
    }
}