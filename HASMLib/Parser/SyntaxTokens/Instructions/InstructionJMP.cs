using HASMLib.Core;
using HASMLib.Runtime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionJMP : Instruction
    {
        public InstructionJMP(int index)
        {
            Index = (UInt24)index;

            Name = new Regex("^[Jj][Mm][Pp]");
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.ConstantOrRegister
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var localIndex = (UInt24)(GetConst(constants, parameters[0].Index).Constant.Value);
            var globalIndex = runtimeMachine.GetGlobalInstructionIndexByLocalOne(localIndex);

            runtimeMachine.CurrentPosition = (UInt24)globalIndex;

            return RuntimeOutputCode.OK;
        }
    }
}
