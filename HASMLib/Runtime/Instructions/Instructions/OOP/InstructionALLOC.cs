using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionALLOC : Instruction
    {
        public InstructionALLOC(int index)
        {
            Index = index;

            NameString = "alloc";
            Name = new Regex("^alloc", RegexOptions.IgnoreCase);
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.NewVariable,
                InstructionParameterType.ClassName
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            return RuntimeOutputCode.OK;
        }
    }
}
