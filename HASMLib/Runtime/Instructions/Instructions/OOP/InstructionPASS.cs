using HASMLib.Parser;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionPASS : Instruction
    {
        public InstructionPASS(int index)
        {
            Index = index;

            NameString = "pass";
            Name = new Regex("^pass", RegexOptions.IgnoreCase);
            ParameterCount = 0;
            ParameterTypes = new List<InstructionParameterType>() { };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            if (package.CallStackItem.RunningFunction.IsConstuctor)
            {
                package.MemZone.ObjectStackItem = package.CallStackItem.Locals[0].Value; //self
            }

            package.RuntimeMachine.Return();

            return RuntimeOutputCode.OK;
        }
    }
}
