using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures
{
    public class Object
    {
        public TypeReference Type;

        public Integer IntegerValue;
        public Array ArrayValue;

        public Dictionary<int, Object> ClassFields;
        public bool IsNull;

        public void InitClassObject()
        {
            foreach (var field in Type.ClassType.Fields)
            {
                ClassFields.Add(field.UniqueID, new Object(field.Type));
            }
            IsNull = false;
        }

        public void SetClassField(int UniqueID, Object value)
        {
            ClassFields[UniqueID] = value;
        }

        public Object GetClassField(int UniqueID)
        {
            return ClassFields[UniqueID];
        }

        public Object(Integer integer, Assembly assembly)
        {
            TypeReference type = new TypeReference(integer.Type, assembly);
            Type = type;

            IntegerValue = integer;
        }

        public Object(TypeReference type)
        {
            Type = type;
            if (Type.Type == TypeReferenceType.Integer)
            {
                IntegerValue = new Integer(0, type.IntegerType);
                IsNull = false;
            }
            else if (Type.Type == TypeReferenceType.Array)
            {
                ArrayValue = new Array(Type.ArrayType);
                IsNull = false;
            }
            else if (Type.Type == TypeReferenceType.Class)
            {
                ClassFields = new Dictionary<int, Object>();
                IsNull = true;
            }
            else throw new System.ArgumentException();
        }

        public override string ToString()
        {
            return $"Object[Type:{Type}]";
        }
    }
}
