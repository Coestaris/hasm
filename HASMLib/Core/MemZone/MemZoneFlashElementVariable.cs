﻿using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using System.Collections.Generic;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementVariable : MemZoneFlashElement
    {
		public UIntDouble Index;
		public LengthQualifier VariableType;

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Variable;
		public override int FixedSize => (1 + 3 + 1) * 8 / (int)HASMBase.Base;

		public override byte[] ToBytes ()
		{
			// 1. (1 byte)  - is: const (0), var (1) or instruction (2)
			// 2. (3 bytes) - var global index
			// 3. (1 byte)  - var length
			List<byte> bytes = new List<byte>();
			bytes.Add(Element_Var);				    //Its a var
			bytes.AddRange(Index.ToBytes());	    //variable index
			bytes.Add((byte)VariableType);			//variable type
			return bytes.ToArray();
		}

		public MemZoneFlashElementVariable(UIntDouble index, LengthQualifier type)
		{
			Index = index;
			VariableType = type;
		}
    }
}