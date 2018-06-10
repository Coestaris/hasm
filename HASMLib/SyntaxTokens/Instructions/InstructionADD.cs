﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HASMLib.Core;

namespace HASMLib.SyntaxTokens.Instructions
{
    public class InstructionADD : Instruction
    {
        public InstructionADD(int index)
        {
            Index = (UInt24)index;

            Name = new Regex("^[Aa][Dd][Dd]");
            ParameterCount = 2;
            ParameterTypes  = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register,
                InstructionParameterType.ConstantOrRegister
            };
        }

        public override void Apply(MemZone memZone, List<InstructionParameter> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
