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

            NameString = "jmp";
            Name = new Regex("^[Jj][Mm][Pp]");
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register | InstructionParameterType.Register | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            UInt24 position = 0;

            if (parameters[0].Type == ReferenceType.Constant)
                position = (UInt24)GetConst(constants, parameters[0].Index).Constant.Value;
            else position = (UInt24)GetVar(memZone, parameters[0].Index).GetNumericValue();

            RuntimeMachineJump(position, runtimeMachine);

            return RuntimeOutputCode.OK;
        }
    }
}
