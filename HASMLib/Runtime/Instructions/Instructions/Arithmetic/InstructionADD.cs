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

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            var dest = GetVar(parameters[0].Index, package);
            var source = GetNumericValue(parameters[1], package);

            if (dest.Value.Type.Type != Structures.TypeReferenceType.Integer)
                return RuntimeOutputCode.ExpectedIntegerVariable;

            dest.Value.IntegerValue += source.Value;

            return RuntimeOutputCode.OK;
        }
    }
}
