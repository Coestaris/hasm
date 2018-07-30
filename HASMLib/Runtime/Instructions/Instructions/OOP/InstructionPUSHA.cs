using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionPUSHA : Instruction
    {
        public InstructionPUSHA(int index)
        {
            Index = index;

            NameString = "pusha";
            Name = new Regex("^pusha", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression,
                //InstructionParameterType.ClassName
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            return RuntimeOutputCode.OK;
        }
    }
}
