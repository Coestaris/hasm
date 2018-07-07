using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionPUSH : Instruction
    {
        public InstructionPUSH(int index)
        {
            Index = index;

            NameString = "push";
            Name = new Regex("^push", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes  = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var source = GetNumericValue(0, memZone, constants, expressions, parameters, runtimeMachine);

            memZone.Stack.Push(source.ToUInt12()[0]);

            return RuntimeOutputCode.OK;
        }
    }
}
