using HASMLib.Core.BaseTypes;
using HASMLib.Parser.SyntaxTokens.Constants;
using System.Collections.Generic;

namespace HASMLib.Core.MemoryZone
{
    public class FlashElementConstant : FlashElement
    {
        public Integer Index { get; set; }
        public Integer Value { get; set; }
        public override FlashElementType Type => FlashElementType.Constant;

        public FlashElementConstant(Integer value, Integer index)
        {
            Index = index;
            Value = value;
        }

        public override int FixedSize
        {
            get => 2 * 8 / BaseIntegerType.PrimitiveType.Base + 2 * HASMBase.PrimitiveTypesInCommon;
        }

        public Constant ToConstant()
        {
            return new Constant(Value);
        }

        public override byte[] ToBytes()
        {
            // 1. (1 byte)  - is: const (0), var (1) or instruction (2)
            // 2. (n bytes) - const global index
            // 3. (1 byte)  - length: 1 - single, 2 - double, 3 - quad(n)
            // 4. (n bytes) - data
            List<byte> bytes = new List<byte>();
            bytes.Add(Element_Const);                   // Is Const;
            bytes.AddRange(Index.ToBytes());            // Global Index
            bytes.Add((byte)Value.Type.Base);           // Length of const
            bytes.AddRange(Value.ToBytes());
            return bytes.ToArray();
        }

        public override string ToString()
        {
            return $"Constant[Index: {Index}. Value: {Value}]";
        }
    }
}