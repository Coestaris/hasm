using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionPUSH : Instruction
    {
        public InstructionPUSH(int index)
        {
            Index = index;

            NameString = "push";
            Name = new Regex("^push", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<ConstantMark> constants, List<FlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var source = GetNumericValue(0, memZone, constants, expressions, parameters, runtimeMachine);

            memZone.Stack.Push(source.ToPrimitive()[0]);

            return RuntimeOutputCode.OK;
        }
    }
}
