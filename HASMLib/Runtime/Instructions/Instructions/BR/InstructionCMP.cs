using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionCMP : Instruction
    {
        [Flags]
        public enum ComapreResult
        {
            Equal = 0x1,
            NotEqual = 0x2,
            Less = 0x4,
            Greater = 0x8,
        }

        public InstructionCMP(int index)
        {
            Index = index;

            NameString = "cmp";
            //CP or CMP
            Name = new Regex("^cm?p", RegexOptions.IgnoreCase);
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression,
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            Integer a = GetNumericValue(parameters[0], package).Value;
            Integer b = GetNumericValue(parameters[1], package).Value;

            ComapreResult result = 0;

            if (a == b) result |= ComapreResult.Equal;
            else result |= ComapreResult.NotEqual;

            if (a > b) result |= ComapreResult.Greater;
            if (a < b) result |= ComapreResult.Less;

            package.MemZone.Stack.Push(BaseIntegerType.PrimitiveType.Cast((Integer)(int)result)[0]);

            return RuntimeOutputCode.OK;
        }
    }
}
