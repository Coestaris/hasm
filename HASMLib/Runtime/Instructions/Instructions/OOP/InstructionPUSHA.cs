using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionPUSHA : Instruction
    {
        public InstructionPUSHA(int index)
        {
            Index = index;

            NameString = "pusha";
            Name = new Regex("^pusha", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression,
                //InstructionParameterType.ClassName
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            if (parameters[0].Type == ReferenceType.Variable)
            {
                package.MemZone.ParamStack.Push(GetVar(parameters[0].Index, package).Value);
            }
            else
            {
                var numericValue = GetNumericValue(parameters[0], package);
                package.MemZone.ParamStack.Push(new Structures.Object(numericValue.Value, package.Assembly));
            }

            return RuntimeOutputCode.OK;


        }
    }
}
