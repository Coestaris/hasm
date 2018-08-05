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

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            var source = GetNumericValue(parameters[0], package);

            //package.MemZone.Stack.Push(source.ToPrimitive()[0]);

            return RuntimeOutputCode.OK;
        }
    }
}
