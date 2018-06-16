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

        public override void Apply(MemZone memZone, List<HASMParser.NamedConstant> constants, List<InstructionParameter> parameters, RuntimeMachine runtimeMachine)
        {
            throw new NotImplementedException();
        }
    }
}
