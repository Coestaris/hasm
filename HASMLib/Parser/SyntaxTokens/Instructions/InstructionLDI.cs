using HASMLib.Core;
using HASMLib.Runtime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionLDI : Instruction
    {
        public InstructionLDI(int index)
        {
            Index = (UInt24)index;

            NameString = "ldi";
            Name = new Regex("^[Ll][Dd][Ii]");
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register,
                InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var dest    = GetVar(memZone, parameters[0].Index);
            var source  = GetConst(constants, parameters[1].Index);

            dest.SetValue(source.Constant.Value);

            return RuntimeOutputCode.OK;
        }
    }
}
