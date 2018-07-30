using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionRET : Instruction
    {
        public InstructionRET(int index)
        {
            Index = index;

            NameString = "ret";
            Name = new Regex("^ret", RegexOptions.IgnoreCase);
            ParameterCount = 0;
            ParameterTypes = new List<InstructionParameterType>() { };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            return RuntimeOutputCode.OK;
        }
    }
}
