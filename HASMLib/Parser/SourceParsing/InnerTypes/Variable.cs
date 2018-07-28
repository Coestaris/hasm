using HASMLib.Core.MemoryZone;
using HASMLib.Runtime.Structures;
using System;

namespace HASMLib.Parser
{
    public class Variable
    {
        public MemZoneFlashElementVariable FEReference { get; internal set; }
        public string Name { get; private set; }
        public TypeReference Type{ get; private set; }

        public Variable(string name, TypeReference type)
        {
            Name = name;
            Type = type;
        }

        public Variable(string name)
        {
            Name = name;
        }
    }
}


