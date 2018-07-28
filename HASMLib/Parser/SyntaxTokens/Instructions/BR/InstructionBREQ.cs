using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.SyntaxTokens.Instructions.InstructionCMP;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionBREQ : Instruction
    {
        public InstructionBREQ(int index)
        {
            Index = index;

            NameString = "breq";
            Name = new Regex("^breq", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            ComapreResult result = (ComapreResult)(int)memZone.Stack.Pop();

            if (result.HasFlag(ComapreResult.Equal))
            {
                Integer position = GetNumericValue(0, memZone, constants, expressions, parameters, runtimeMachine).Value;

                RuntimeMachineJump(position, runtimeMachine);
            }

            return RuntimeOutputCode.OK;
        }
    }
}
