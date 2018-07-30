using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionNOP : Instruction
    {
        public InstructionNOP(int index)
        {
            Index = index;

            NameString = "nop";
            Name = new Regex("^nop", RegexOptions.IgnoreCase);
            ParameterCount = 0;
            ParameterTypes = new List<InstructionParameterType>() { };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            return RuntimeOutputCode.OK;
        }
    }
}
