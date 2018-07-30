using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionADD : Instruction
    {
        public InstructionADD(int index)
        {
            Index = index;

            NameString = "add";
            Name = new Regex("^add", RegexOptions.IgnoreCase);
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable,
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<ConstantMark> constants, List<FlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var dest = GetVar(memZone, parameters[0].Index);
            var source = GetNumericValue(1, memZone, constants, expressions, parameters, runtimeMachine);

            //dest.Value += source.Value;

            return RuntimeOutputCode.OK;
        }
    }
}
