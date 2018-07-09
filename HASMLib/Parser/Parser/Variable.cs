using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class Variable
    {
        public MemZoneFlashElementVariable FEReference { get; internal set; }
        public string Name { get; private set; }
        public LengthQualifier Length { get; private set; }

        public Variable(string name, LengthQualifier length)
        {
            Name = name;
            Length = length;
        }
    }
}


