using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using HASMLib.Runtime.Structures.Units;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionCALL : Instruction
    {
        public InstructionCALL(int index)
        {
            Index = index;

            NameString = "call";
            Name = new Regex("^call", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.FunctionName
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            Integer id = parameters[0].Index;
            Function function = package.Assembly.AllFunctions.Find(p => (Integer)p.UniqueID == id);

            if(function.IsStatic)
            {

            }
            else
            {
				RuntimeOutputCode error;
                if (!CheckObjectStackItem(package, function.BaseClass, out error))
                    return error;

                if (function.IsConstuctor)
                {
                    package.MemZone.ObjectStackItem.InitClassObject();
                }
            }

            var result = package.RuntimeMachine.CallFunction(function);
            if (result != null) return result.Code;

            return RuntimeOutputCode.OK;
        }
    }
}
