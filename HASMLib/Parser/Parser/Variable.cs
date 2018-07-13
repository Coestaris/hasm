using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class Variable
    {
        public MemZoneFlashElementVariable FEReference { get; internal set; }
        public string Name { get; private set; }
        public int Base { get; private set; }

        public Variable(string name, int _base)
        {
            Name = name;
            Base = _base;
        }
    }
}


