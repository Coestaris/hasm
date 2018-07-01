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
            Index = (UInt24)index;

            NameString = "jmp";
            Name = new Regex("^jmp", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register | InstructionParameterType.Register | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            UInt24 position = (UInt24)GetNumericValue(0, memZone, constants, expressions, parameters, runtimeMachine).Value;

            RuntimeMachineJump(position, runtimeMachine);

            return RuntimeOutputCode.OK;
        }
    }
}
