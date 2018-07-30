using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionPOP : Instruction
    {
        public InstructionPOP(int index)
        {
            Index = index;

            NameString = "pop";
            Name = new Regex("^pop", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            var dest = GetVar(parameters[0].Index, package);

            throw new NotImplementedException();

            //dest.Value = memZone.Stack.Pop();

            return RuntimeOutputCode.OK;
        }
    }
}
