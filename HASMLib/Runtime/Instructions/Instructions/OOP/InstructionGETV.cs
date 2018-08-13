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

            if (package.MemZone.ObjectStackItem.Type.Type != Structures.TypeReferenceType.Class)
                return RuntimeOutputCode.ClassTypeExpected;

            if (package.MemZone.ObjectStackItem.Type.ClassType != field.BaseClass)
                return RuntimeOutputCode.DifferentClasses;

            package.MemZone.ObjectStackItem = package.MemZone.ObjectStackItem.GetClassField(field.UniqueID);

            return RuntimeOutputCode.OK;
        }
    }
}
