using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures;
using System.Collections.Generic;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementVariable : MemZoneFlashElement
    {
        public Integer Index { get; private set; }
        public TypeReference VariableType { get; private set; }

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Variable;
        public override int FixedSize => 2 * 8 / BaseIntegerType.PrimitiveType.Base + HASMBase.PrimitiveTypesInCommon;

        public override byte[] ToBytes()
        {
            // 1. (1 byte)  - is: const (0), var (1) or instruction (2)
            // 2. (3 bytes) - var global index
            // 3. (1 byte)  - var length
            List<byte> bytes = new List<byte>();
            bytes.Add(Element_Var);                 //Its a var
            bytes.AddRange(Index.ToBytes());        //variable index
            //bytes.Add((byte)VariableType.Base);         //variable type
            return bytes.ToArray();
        }

        public MemZoneFlashElementVariable(Integer index, TypeReference type)
        {
            Index = index;
            VariableType = type;
        }

        public override string ToString()
        {
            return $"Variable[Index: {Index}, Type: {VariableType}]";
        }
    }
}