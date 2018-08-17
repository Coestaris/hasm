using HASMLib.Parser;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Runtime.Instructions.Instructions
{
    public class InstructionGETV : Instruction
    {
        public InstructionGETV(int index)
        {
            Index = index;

            NameString = "getv";
            Name = new Regex("^getv", RegexOptions.IgnoreCase);
            ParameterCount = 1;
            ParameterTypes = new List<InstructionParameterType>()
            {
                InstructionParameterType.FieldName
            };
        }

        public override RuntimeOutputCode Apply(RuntimeDataPackage package, List<ObjectReference> parameters)
        {
            Field field = package.Assembly.AllFields.Find(p => p.UniqueID == (int)parameters[0].Index);

            if(field.IsStatic)
            {
                package.MemZone.ObjectStackItem = field.BaseClass.StaticFields[field.UniqueID];
            }
            else
            {
                if (!CheckObjectStackItem(package, field.BaseClass, out RuntimeOutputCode error))
                    return error;

                package.MemZone.ObjectStackItem = package.MemZone.ObjectStackItem.GetClassField(field.UniqueID);
            }

            return RuntimeOutputCode.OK;
        }
    }
}
