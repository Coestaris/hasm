using HASMLib.Core;
using HASMLib.Runtime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionMOV : Instruction
    {
        public InstructionMOV(int index)
        {
            Index = (UInt24)index;

            Name = new Regex("^[Mm][Oo][Vv]");
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register,
                InstructionParameterType.Register
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var dest    = GetVar(memZone, parameters[0].Index);
            var source  = GetVar(memZone, parameters[1].Index);

            dest.SetValue(source);

            return RuntimeOutputCode.OK;
        }
    }
}
