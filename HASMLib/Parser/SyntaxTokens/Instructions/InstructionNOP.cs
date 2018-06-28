using HASMLib.Core;
using HASMLib.Runtime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionNOP : Instruction
    {
        public InstructionNOP(int index)
        {
            Index = (UInt24)index;

            NameString = "nop";
            Name = new Regex("^[Nn][Oo][Pp]");
            ParameterCount = 0;
            ParameterTypes = new List<InstructionParameterType>() { };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            return RuntimeOutputCode.OK;
        }
    }
}
