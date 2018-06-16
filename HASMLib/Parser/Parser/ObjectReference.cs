using HASMLib.Core;

namespace HASMLib.Parser
{
    public partial class HASMParser
	{
        public struct ObjectReference
        {
            public UInt24 Index;
            public ReferenceType Type;

            public ObjectReference(UInt24 index, ReferenceType type)
            {
                Index = index;
                Type = type;
            }
        }
	}
}


