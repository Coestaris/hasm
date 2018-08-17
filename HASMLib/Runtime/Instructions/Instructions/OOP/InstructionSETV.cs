using HASMLib.Parser;
using HASMLib.Runtime.Structures;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionSETV : Instruction
    {
        public InstructionSETV(int index)
        {
            Index = index;

            NameString = "setv";
            Name = new Regex("^setv", RegexOptions.IgnoreCase);
            ParameterCount = 2;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.FieldName,
                InstructionParameterType.Variable | InstructionParameterType.Constant | InstructionParameterType.Expression
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            Field field = package.Assembly.AllFields.Find(p => p.UniqueID == (int)parameters[0].Index);
            Object obj = GetObject(parameters[1], package);

            if (obj.Type != field.Type)
                return RuntimeOutputCode.DifferentTypes;

            if (field.IsStatic)
            {
                field.BaseClass.StaticFields[field.UniqueID] = obj;
            }
            else
            {
                if (!CheckObjectStackItem(package, field.BaseClass, out RuntimeOutputCode error))
                    return error;

                package.MemZone.ObjectStackItem.SetClassField(field.UniqueID, obj);
            }
            return RuntimeOutputCode.OK;
        }
    }
}
