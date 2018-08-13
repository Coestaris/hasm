using HASMLib.Parser;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionRET : Instruction
    {
        public InstructionRET(int index)
        {
            Index = index;

            NameString = "ret";
            Name = new Regex("^ret", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression,
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            if (package.CallStackItem.RunningFunction.IsConstuctor)
                return RuntimeOutputCode.ReturnInConstructorsAreNotAllowed;

            package.RuntimeMachine.Return();

            package.MemZone.ObjectStackItem = GetObject(parameters[0], package);

            return RuntimeOutputCode.OK;
        }
    }
}
