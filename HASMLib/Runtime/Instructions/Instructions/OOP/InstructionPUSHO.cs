using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionPUSHO : Instruction
    {
        public InstructionPUSHO(int index)
        {
            Index = index;

            NameString = "pusho";
            Name = new Regex("^pusho", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            Integer varIndex = parameters[0].Index;

            package.MemZone.ObjectStackItem = GetVar(varIndex, package).Value;

            return RuntimeOutputCode.OK;
        }
    }
}
