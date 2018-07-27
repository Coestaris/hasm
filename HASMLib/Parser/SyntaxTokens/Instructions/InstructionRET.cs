using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Instructions
{
    public class InstructionRET : Instruction
    {
        public InstructionRET(int index)
        {
            Index = index;

            NameString = "ret";
            Name = new Regex("^ret", RegexOptions.IgnoreCase);
            ParameterCount = 0;
            ParameterTypes = new List<InstructionParameterType>() { };
        }

        public override RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<MemZoneFlashElementExpression> expressions, List<ObjectReference> parameters, RuntimeMachine runtimeMachine)
        {
            return RuntimeOutputCode.OK;
        }
    }
}
