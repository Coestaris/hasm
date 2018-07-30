using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
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
                InstructionParameterType.Constant | InstructionParameterType.Variable | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            Integer position = GetNumericValue(parameters[0], package).Value;

            //RuntimeMachineJump(position, runtimeMachine);

            return RuntimeOutputCode.OK;
        }
    }
}
