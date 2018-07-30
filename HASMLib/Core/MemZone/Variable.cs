using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures;

namespace HASMLib.Core.MemoryZone
{
    public class Variable
    {
        public string Name { get; protected set; }
        public Integer Index { get; protected set; }
        public Object Value { get; set; }

        public Variable(TypeReference type, Integer index) : this(new Object(type), index)
        { }

        public Variable(Object value, Integer index)
        {
            Value = value;
            Index = index;
        }

        public void AddValue(Variable value)
        {
            //Value += value.Value;
        }

        public void SetValue(Variable value)
        {
            Value = value.Value;
        }

        public void SetValue(ulong value)
        {
            //Value = new Integer(value, Value.Type);
        }

        public void AddValue(ulong value)
        {
            //Value = new Integer(Value.Value + value, Value.Type);
        }
    }
}