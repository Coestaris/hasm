using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Runtime.Instructions.Instructions.InstructionCMP;

namespace HASMLib.Runtime.Instructions.Instructions
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

        public override RuntimeOutputCode Apply(MemZone memZone, List<ConstantMark> constants, List<FlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
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
