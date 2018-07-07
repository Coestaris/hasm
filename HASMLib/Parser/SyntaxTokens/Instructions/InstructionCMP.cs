using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Instructions
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
                InstructionParameterType.Register | InstructionParameterType.Constant | InstructionParameterType.Expression,
                InstructionParameterType.Register | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            long a = GetNumericValue(0, memZone, constants, expressions, parameters, runtimeMachine).Value;
            long b = GetNumericValue(1, memZone, constants, expressions, parameters, runtimeMachine).Value;

            ComapreResult result = 0;

            if (a == b) result |= ComapreResult.Equal;
            else result |= ComapreResult.NotEqual;

            if (a > b) result |= ComapreResult.Greater;
            if (a < b) result |= ComapreResult.Less;

            memZone.Stack.Push((UIntSingle)(int)result);

            return RuntimeOutputCode.OK;
        }
    }
}
