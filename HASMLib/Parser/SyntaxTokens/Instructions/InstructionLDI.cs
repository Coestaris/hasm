using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HASMLib.Core;
using HASMLib.Runtime;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionLDI : Instruction
    {
        public InstructionLDI(int index)
        {
            Index = (UInt24)index;

            Name = new Regex("^[Ll][Dd][Ii]");
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register,
                InstructionParameterType.Constant
            };
        }

        public override void Apply(MemZone memZone, List<HASMParser.NamedConstant> constants, List<InstructionParameter> parameters, RuntimeMachine runtimeMachine)
        {
            throw new NotImplementedException();
        }
    }
}
