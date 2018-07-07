using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionJMP : Instruction
    {
        public InstructionJMP(int index)
        {
            Index = index;

            NameString = "jmp";
            Name = new Regex("^jmp", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Constant | InstructionParameterType.Register | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            UIntDouble position = (UIntDouble)GetNumericValue(0, memZone, constants, expressions, parameters, runtimeMachine).Value;

            RuntimeMachineJump(position, runtimeMachine);

            return RuntimeOutputCode.OK;
        }
    }
}
