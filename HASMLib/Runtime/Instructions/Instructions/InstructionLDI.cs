using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionLDI : Instruction
    {
        public InstructionLDI(int index)
        {
            Index = index;

            NameString = "ldi";
            Name = new Regex("^ldi", RegexOptions.IgnoreCase);
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable,
                InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            var dest = GetVar(parameters[0].Index, package);
            var source = GetNumericValue(parameters[0], package);

            throw new NotImplementedException();

            //dest.Value = source.Value;

            return RuntimeOutputCode.OK;
        }
    }
}
