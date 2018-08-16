using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures;

namespace HASMLib.Parser
{
    public class VariableMark
    {
        public Integer Index { get; set; }
        public string Name { get; private set; }
        public TypeReference Type{ get; private set; }

        public VariableMark(string name, TypeReference type)
        {
            Name = name;
            Type = type;
        }

        public VariableMark(string name)
        {
            Name = name;
        }
    }
}


