using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionPOP : Instruction
    {
        public InstructionPOP(int index)
        {
            Index = index;

            NameString = "pop";
            Name = new Regex("^pop", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes  = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var dest = GetVar(memZone, parameters[0].Index);

            dest.SetValue(memZone.Stack.Pop());

            return RuntimeOutputCode.OK;
        }
    }
}
