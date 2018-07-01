using HASMLib.Core;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class ObjectReference
    {
        public UInt24 Index;
        public ReferenceType Type;

        public MemZoneFlashElement Object;

        public ObjectReference(UInt24 index, ReferenceType type)
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


