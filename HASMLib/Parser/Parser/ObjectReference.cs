using HASMLib.Core;

namespace HASMLib.Parser
{
    internal partial class HASMParser
	{
        internal struct ObjectReference
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


