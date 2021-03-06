﻿using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionOUT : Instruction
    {
        public InstructionOUT(int index)
        {
            Index = index;

            NameString = "out";
            Name = new Regex("^out", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Constant | InstructionParameterType.Expression | InstructionParameterType.Variable
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            var value = GetNumericValue(parameters[0], package);

            if (value == null)
                return RuntimeOutputCode.ExpectedIntegerVariable;

            if (value.Type == Structures.TypeReferenceType.Integer)
                package.RuntimeMachine.OutBytes("stdout", value.ToPrimitiveInt());
            else
                package.RuntimeMachine.OutBytes("stdout", value.ToPrimitiveArray());
            return RuntimeOutputCode.OK;
        }
    }
}
