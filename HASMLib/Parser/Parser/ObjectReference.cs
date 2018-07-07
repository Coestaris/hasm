using HASMLib.Core;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class ObjectReference
    {
        public UIntDouble Index;
        public ReferenceType Type;

        public MemZoneFlashElement Object;

        public ObjectReference(UIntDouble index, ReferenceType type)
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


