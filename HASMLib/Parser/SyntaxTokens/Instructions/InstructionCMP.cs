using HASMLib.Core;
using HASMLib.Runtime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;

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
            Index = (UInt24)index;

            NameString = "cmp";
            //CP or CMP
            Name = new Regex("^[Cc]([Mm])?[Pp]");
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.ConstantOrRegister,
                InstructionParameterType.ConstantOrRegister
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            long a = 0;
            long b = 0;

            if (parameters[0].Type == ReferenceType.Constant)
                a = GetConst(constants, parameters[0].Index).Constant.Value;
            else a = GetVar(memZone, parameters[0].Index).GetNumericValue();

            if (parameters[1].Type == ReferenceType.Constant)
                b = GetConst(constants, parameters[1].Index).Constant.Value;
            else b = GetVar(memZone, parameters[1].Index).GetNumericValue();

            ComapreResult result = 0;

            if (a == b) result |= ComapreResult.Equal;
            else result |= ComapreResult.NotEqual;

            if (a > b) result |= ComapreResult.Greater;
            if (a < b) result |= ComapreResult.Less;

            memZone.Stack.Push((UInt12)(int)result);

            return RuntimeOutputCode.OK;
        }
    }
}
