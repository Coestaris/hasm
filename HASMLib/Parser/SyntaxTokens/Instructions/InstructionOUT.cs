﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HASMLib.Core;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionOUT : Instruction
    {
        public InstructionOUT(int index)
        {
            Index = (UInt24)index;

            Name = new Regex("^[Oo][Uu][Tt]");
            ParameterCount = 1;
            ParameterTypes  = new List<InstructionParameterType>()
            {
                InstructionParameterType.ConstantOrRegister
            };
        }

        public override void Apply(MemZone memZone, List<InstructionParameter> parameters)
        {
            throw new NotImplementedException();
        }
    }
}