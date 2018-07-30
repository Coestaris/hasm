using HASMLib.Core.MemoryZone;
using HASMLib.Runtime.Structures;
using System;

namespace HASMLib.Parser
{
    public class VariableMark
    {
        public FlashElementVariable FEReference { get; internal set; }
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


