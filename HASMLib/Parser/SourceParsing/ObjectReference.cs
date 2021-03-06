﻿using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    public class ObjectReference
    {
        public Integer Index { get; internal set; }
        public ReferenceType Type { get; internal set; }
        public FlashElement Object { get; internal set; }

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


