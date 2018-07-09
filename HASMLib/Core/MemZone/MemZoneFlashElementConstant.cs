using System.Collections.Generic;
using HASMLib.Parser.SyntaxTokens;
using HASMLib.Core.BaseTypes;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementConstant : MemZoneFlashElement
    {
		public int Index;

		protected LengthQualifier Length { get; set; }
        protected FSingle[] Value { get; set; }

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Constant;
		public override int FixedSize 
		{
			get
			{
				int a = 5; // 1 + 3 + 1
				switch (Length) {
					case LengthQualifier.Single : a += 2; break; 
					case LengthQualifier.Double : a += 3; break; 
					case LengthQualifier.Quad   : a += 8; break;
				}
				return a * 8 / (int)HASMBase.Base; //To get N-bit representation
			}
		}

        public Constant ToConstant()
        {
            switch (Length)
            {
                case LengthQualifier.Single : return new Constant(Value[0], LengthQualifier.Single);
                case LengthQualifier.Double : return new Constant(FDouble.FromSingle(Value), LengthQualifier.Double);
                case LengthQualifier.Quad   : return new Constant(FQuad.FromSingle(Value), LengthQualifier.Quad);
                default                     : return null;
            }
        }

		public override byte[] ToBytes ()
		{
			// 1. (1 byte)  - is: const (0), var (1) or instruction (2)
			// 2. (3 bytes) - const global index
			// 3. (1 byte)  - length: 1 - single, 2 - double, 3 - quad(n)
			// 4. (n bytes) - data
			List<byte> bytes = new List<byte>();
			bytes.Add(Element_Const);					// Is Const;
			bytes.AddRange(((FDouble)Index).ToBytes()); 	// Global Index
			bytes.Add((byte)Length);							// Length of const
			foreach (var item in Value) {
				bytes.AddRange (item.ToBytes());		// Data
			}
			return bytes.ToArray();
		}
    }
}