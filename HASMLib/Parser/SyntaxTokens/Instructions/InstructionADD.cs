using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Parser.SyntaxTokens.Instructions
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

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var dest = GetVar(memZone, parameters[0].Index);

            if (parameters[1].Type == ReferenceType.Variable)
            {
                var source = GetVar(memZone, parameters[1].Index);
                dest.AddValue(source);
            }
            else
            {
                var source = GetConst(constants, parameters[1].Index);
                dest.AddValue(source.Constant.Value);
            }

            return RuntimeOutputCode.OK;
        }
    }
}
