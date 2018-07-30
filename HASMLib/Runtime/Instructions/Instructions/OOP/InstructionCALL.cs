using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionCALL : Instruction
    {
        public InstructionCALL(int index)
        {
            Index = index;

            NameString = "call";
            Name = new Regex("^call", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.FunctionName
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            return RuntimeOutputCode.OK;
        }
    }
}
