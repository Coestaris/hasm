using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using HASMLib.Runtime.Structures;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionALLOC : Instruction
    {
        public InstructionALLOC(int index)
        {
            Index = index;

            NameString = "alloc";
            Name = new Regex("^alloc", RegexOptions.IgnoreCase);
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.NewVariable,
                InstructionParameterType.ClassName
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            Integer varIndex = parameters[0].Index;
            Integer classIndex = parameters[1].Index;

            TypeReference type = package.Assembly.UsedTypes.Find(p => p.UniqueID == (int)classIndex);

            package.MemZone.RAM.Add(new Variable(type, varIndex));

            return RuntimeOutputCode.OK;
        }
    }
}
