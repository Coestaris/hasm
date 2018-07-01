using HASMLib.Core;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class Variable
    {
        public MemZoneFlashElementVariable variable;

        public string Name;
        public LengthQualifier Length;

        public Variable(string name, LengthQualifier length)
        {
            Name = name;
            Length = length;
        }
    }
}


