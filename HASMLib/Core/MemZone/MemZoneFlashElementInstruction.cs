using HASMLib.Core.BaseTypes;
using HASMLib.Parser;
using HASMLib.Parser.SyntaxTokens;
using System.Collections.Generic;

namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneFlashElementInstruction : MemZoneFlashElement
    {
        public Integer InstructionNumber { get; private set; }
        public List<ObjectReference> Parameters { get; private set; }
        public Integer ProgramIndex { get; private set; }
        public Integer RuntimeAbsoluteIndex { get; internal set; }
        
        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Instruction;

        public override int FixedSize
        {
            get
            {
                if (Parameters == null) return (4 + 2) * 8 / (int)HASMBase.Base;
                else return (4 + 2 + 3 * Parameters.Count) * 8 / (int)HASMBase.Base;
            }
        }

        private const byte Parameter_Const = 1;
        private const byte Parameter_Var = 2;

        public override byte[] ToBytes()
        {
            // 1. (1 byte)  	- is: const (0), var (1) or instruction (2)
            // 2. (3 bytes) 	- Instruction number
            // 3. (3 bytes)     - Instrcution index
            // 3. (3 * n byte)	- Arguments: 1st byte - (1) or (2): constant or variable
            //					- 2st and 3d bytes: index of constant of variable
            List<byte> bytes = new List<byte>();

            bytes.Add(Element_Instruction);                             //Its a instrucion
            bytes.AddRange(InstructionNumber.ToBytes());                //Instrucion number
            if (Parameters == null)
                return bytes.ToArray();

            bytes.AddRange(ProgramIndex.ToBytes());                     //Instrucion index

            foreach (var item in Parameters)
            {
                bytes.Add(item.Type == ReferenceType.Constant ? Parameter_Const : Parameter_Var);    //Const or variable
                bytes.AddRange(item.Index.ToBytes());                       //Index of argument
            }

            return bytes.ToArray();
        }

        public MemZoneFlashElementInstruction(Instruction instruction, List<ObjectReference> arguments, Integer index)
        {
            InstructionNumber = (Integer)instruction.Index;
            Parameters = arguments;
            ProgramIndex = index;
        }
    }
}