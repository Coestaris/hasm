using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class ObjectReference
    {
        public Integer Index { get; private set; }
        public ReferenceType Type { get; private set; }
        public MemZoneFlashElement Object { get; internal set; }

        public ObjectReference(Integer index, ReferenceType type)
        {
            Index = index;
            Type = type;
        }

        public override string ToString()
        {
            return $"Type: {Type}. Index: {Index}";
        }
    }
}


