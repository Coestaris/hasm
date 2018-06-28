using HASMLib.Core;
using HASMLib.Runtime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionOUT : Instruction
    {
        public InstructionOUT(int index)
        {
            Index = (UInt24)index;

            NameString = "out";
            Name = new Regex("^[Oo][Uu][Tt]");
            ParameterCount = 1;
            ParameterTypes  = new List<InstructionParameterType>()
            {
                InstructionParameterType.ConstantOrRegister
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            if(parameters[0].Type == ReferenceType.Constant)
            {
                var source = GetConst(constants, parameters[0].Index);

                runtimeMachine.OutBytes(source.Constant.ToUInt12());
            }
            else
            {
                var source = GetVar(memZone, parameters[0].Index);

                runtimeMachine.OutBytes(source.ToUInt12());
            }

            return RuntimeOutputCode.OK;
        }
    }
}
