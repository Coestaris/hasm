using HASMLib.Core;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;
using static HASMLib.Parser.SyntaxTokens.Instructions.InstructionCMP;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionBREQ : Instruction
    {
        public InstructionBREQ(int index)
        {
            Index = (UInt24)index;

            NameString = "breq";
            Name = new Regex("^[Bb][Rr][Ee][Qq]");
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.ConstantOrRegister
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            ComapreResult result = (ComapreResult)(int)memZone.Stack.Pop();

            if(result.HasFlag(ComapreResult.Equal))
            {
                UInt24 position = 0;

                if (parameters[0].Type == ReferenceType.Constant)
                    position = (UInt24)GetConst(constants, parameters[0].Index).Constant.Value;
                else position = (UInt24)GetVar(memZone, parameters[0].Index).GetNumericValue();

                RuntimeMachineJump(position, runtimeMachine);
            }

            return RuntimeOutputCode.OK;
        }
    }
}
