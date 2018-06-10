using HASMLib.SyntaxTokens;
using System;

namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneFlashElementInstruction : MemZoneFlashElement
    {
        private UInt24 InstructionNumber;
        private object[] Parameters;

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Instruction;

        public override int FixedSize 
        {
            get
            {
                //Index of instruction
                int size = 2;

                if (Parameters == null)
                    return size;

                foreach (var a in Parameters)
                    if (a.GetType() == typeof(UInt24)) size += 2;
                    else size += ((string)a).Length;

                return size;
            }
        }

        public MemZoneFlashElementInstruction(Instruction instruction, params object[] arguments)
        {
            InstructionNumber = instruction.Index;
            foreach (var item in arguments)
            {
                if (item.GetType() != typeof(UInt24) && item.GetType() != typeof(string))
                    throw new ArgumentException();
            }
            Parameters = arguments;
        }
    }
}