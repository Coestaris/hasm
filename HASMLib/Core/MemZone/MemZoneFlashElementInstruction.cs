using HASMLib.Parser.SyntaxTokens;
using System.Linq;
using System.Collections.Generic;
using System;

namespace HASMLib.Core.MemoryZone
{
    internal class MemZoneFlashElementInstruction : MemZoneFlashElement
    {
        private UInt24 InstructionNumber;
		private List<Tuple<UInt24, bool>> Parameters;

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Instruction;

        public override int FixedSize 
        {
            get 
            {
				if(Parameters == null) return 4;
				else return (4 + 3 * Parameters.Count) * 8 / 12; //To get 12-representation of 8-bit
            }
        }

		private const byte Parameter_Const = 1;
		private const byte Parameter_Var   = 2;

		public override byte[] ToBytes ()
		{
			// 1. (1 byte)  	- is: const (0), var (1) or instruction (2)
			// 2. (3 bytes) 	- Instruction number
			// 3. (3 * n byte)	- Arguments: 1st byte - (1) or (2): constant or variable
			//					-	2st and 3d bytes: index of constant of variable
			List<byte> bytes = new List<byte>();

			bytes.Add (Element_Instruction);								//Its a instrucion
			bytes.AddRange (InstructionNumber.ToBytes ());					//Instrucion number
			if(Parameters == null)
				return bytes.ToArray ();

			foreach (var item in Parameters) {
				bytes.Add (item.Item2 ? Parameter_Const : Parameter_Var);	//Const or variable
				bytes.AddRange(item.Item1.ToBytes ());						//Index of argument
			}

			return bytes.ToArray ();
		}

		public MemZoneFlashElementInstruction(Instruction instruction, List<Tuple<UInt24, bool>> arguments)
        {
			InstructionNumber = instruction.Index;
            Parameters = arguments;
        }
    }
}