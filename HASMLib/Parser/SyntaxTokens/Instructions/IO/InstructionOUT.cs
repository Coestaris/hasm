using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionOUT : Instruction
    {
        public InstructionOUT(int index)
        {
            Index = index;

            NameString = "out";
            Name = new Regex("^out", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Constant | InstructionParameterType.Expression | InstructionParameterType.Variable
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var value = GetNumericValue(0, memZone, constants, expressions, parameters, runtimeMachine);
            runtimeMachine.OutBytes(value.ToPrimitive());
            return RuntimeOutputCode.OK;
        }
    }
}
