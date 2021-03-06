﻿namespace HASMLib.Core.MemoryZone
{
    public abstract class FlashElement
    {
        protected const byte Element_Const = 1;
        protected const byte Element_Var = 2;
        protected const byte Element_Instruction = 3;

        public abstract int FixedSize { get; }
        public abstract FlashElementType Type { get; }

        public abstract byte[] ToBytes();
    }
}