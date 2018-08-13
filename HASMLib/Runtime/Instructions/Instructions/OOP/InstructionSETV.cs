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

            if (package.MemZone.ObjectStackItem.Type.Type != Structures.TypeReferenceType.Class)
                return RuntimeOutputCode.ClassTypeExpected;

            if (package.MemZone.ObjectStackItem.Type.ClassType != field.BaseClass)
                return RuntimeOutputCode.DifferentClasses;

            if(obj.Type != field.Type)
                return RuntimeOutputCode.DifferentTypes;

            package.MemZone.ObjectStackItem.SetClassField(field.UniqueID, obj);

            return RuntimeOutputCode.OK;
        }
    }
}
