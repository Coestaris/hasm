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

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            /*ComapreResult result = (ComapreResult)(int)package.MemZone.Stack.Pop();

            if (result.HasFlag(ComapreResult.Equal))
            {
                Integer position = GetNumericValue(parameters[0], package).Value;

                RuntimeMachineJump(position, package.RuntimeMachine);
            }*/;

            return RuntimeOutputCode.OK;
        }
    }
}
