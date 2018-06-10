using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HASMLib.Core;

namespace HASMLib.SyntaxTokens.Instructions
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

        public override void Apply(MemZone memZone, List<InstructionParameter> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
