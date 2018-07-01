using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionLDI : Instruction
    {
        public InstructionLDI(int index)
        {
            Index = (UInt24)index;

            NameString = "ldi";
            Name = new Regex("^ldi", RegexOptions.IgnoreCase);
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Register,
                InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            var dest    = GetVar(memZone, parameters[0].Index);
            var source  = GetNumericValue(1, memZone, constants, expressions, parameters);

            dest.SetValue(source.Value);

            return RuntimeOutputCode.OK;
        }
    }
}
