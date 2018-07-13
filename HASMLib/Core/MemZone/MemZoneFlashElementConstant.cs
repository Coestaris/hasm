using System.Collections.Generic;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Core.BaseTypes;
using System;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementConstant : MemZoneFlashElement
    {
        public Integer Index { get; set; }
        protected Integer Value { get; set; }
        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Constant;

        public MemZoneFlashElementConstant(Integer value, Integer index)
        {
            Index = index;
            Value = value;
        }

        public override int FixedSize 
		{
			get => (1 + 2 + 1 + HASMBase.Base / BaseIntegerType.PrimitiveType.Base) * 8 / HASMBase.Base;
		}

        public Constant ToConstant()
        {
            return new Constant(Value);
        }

		public override byte[] ToBytes ()
		{
			// 1. (1 byte)  - is: const (0), var (1) or instruction (2)
			// 2. (3 bytes) - const global index
			// 3. (1 byte)  - length: 1 - single, 2 - double, 3 - quad(n)
			// 4. (n bytes) - data
			List<byte> bytes = new List<byte>();
			bytes.Add(Element_Const);                   // Is Const;
            bytes.AddRange(Index.ToBytes()); 	        // Global Index
			bytes.Add((byte)Value.Type.Base);                   // Length of const
            bytes.AddRange(Value.ToBytes());
			return bytes.ToArray();
		}
    }
}