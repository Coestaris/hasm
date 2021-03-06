﻿using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionPOPR : Instruction
    {
        public InstructionPOPR(int index)
        {
            Index = index;

            NameString = "popr";
            Name = new Regex("^popr", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            Integer varIndex = parameters[0].Index;

            var variable = GetVar(varIndex, package);

            if (package.MemZone.ObjectStackItem == null)
                return RuntimeOutputCode.ObjectStackIsEmpty;

            if (package.MemZone.ObjectStackItem.Type != variable.Value.Type)
                return RuntimeOutputCode.DifferentTypes;

            variable.Value = package.MemZone.ObjectStackItem;

            return RuntimeOutputCode.OK;
        }
    }
}
