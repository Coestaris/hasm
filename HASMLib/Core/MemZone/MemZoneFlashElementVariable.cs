﻿using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using System.Collections.Generic;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementVariable : MemZoneFlashElement
    {
		public UInt24 Index;
		public MemZoneVariableLength VariableType;

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Variable;
		public override int FixedSize => 4; //(1 + 3 + 1) * 8 / 12;

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

		public MemZoneFlashElementVariable(UInt24 index, MemZoneVariableLength type)
		{
			Index = index;
			VariableType = type;
		}
    }
}