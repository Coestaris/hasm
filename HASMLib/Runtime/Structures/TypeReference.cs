using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Runtime.Structures
{
    public class TypeReference
    {
        public TypeReferenceType Type = TypeReferenceType.Void;
        public int UniqueID;

        public string Name;

        internal bool Registered;

        public BaseIntegerType IntegerType;
        public Class ClassType;
        public ArrayType ArrayType;

        public static TypeReference Void => new TypeReference();

        public override string ToString()
        {
            switch (Type)
            {
                case TypeReferenceType.Class:
                    return ClassType == null ? $"Unknown reference (class)" : ClassType.FullName;
                case TypeReferenceType.Integer:
                    return IntegerType == null ? $"Unknown reference (integer)" : IntegerType.Name;
                case TypeReferenceType.Array:
                    return ArrayType == null ? $"Unknown reference (array)" : ArrayType.Name;
                case TypeReferenceType.Void:
                default:
                    return Function.NoReturnableValueKeyword;
            }
        }

        public int Size
        {
            get
            {
                if (Type == TypeReferenceType.Integer)
                    return IntegerType.Base / HASMBase.Base;

                if (Type == TypeReferenceType.Array)
                    return ArrayType.BaseType.Size;

                if (Type == TypeReferenceType.Class)
                    return ClassType.Fields.Sum(p => p.Type.Size);

                if (Type == TypeReferenceType.Void)
                    return 0;

                return 0;
            }
        }

        private TypeReference() { }

        internal bool CheckClassType(List<Class> Classes, Assembly assembly)
        {
            if (!Registered) assembly.RegisterType(this);

            if (Type == TypeReferenceType.Void) return true;
            if (Type == TypeReferenceType.Integer) return true;

            if (Type == TypeReferenceType.Class)
            {
                if (ClassType != null) return true;

                Class Class = Classes.Find(p => p.FullName == assembly.ToAbsoluteName(Name));
                if (Class == null) return false;
                Name = Class.FullName;
                ClassType = Class;
                return true;
            }
            else //TypeReferenceType.Array
            {
                return ArrayType.BaseType.CheckClassType(Classes, assembly);
            }
        }

        public override bool Equals(object obj)
        {
			return obj is TypeReference && (obj as TypeReference) == this;
        }

        public override int GetHashCode()
        {
            return 494236115 + UniqueID.GetHashCode();
        }

        public TypeReference(ArrayType arrayType, Assembly assembly)
        {
            Type = TypeReferenceType.Array;
            Name = arrayType.Name;
            ArrayType = arrayType;
            assembly?.RegisterType(this);
        }

        public TypeReference(BaseIntegerType intType, Assembly assembly)
        {
            Type = TypeReferenceType.Integer;
            Name = intType.Name;
            IntegerType = intType;
            assembly?.RegisterType(this);
        }

        public TypeReference(Class Class, Assembly assembly)
        {
            Type = TypeReferenceType.Class;
            Name = Class.FullName;
            ClassType = Class;
            assembly?.RegisterType(this);
        }

        public TypeReference(string fullName, Assembly assembly)
        {
            Name = fullName;
            var bit = BaseIntegerType.Types.Find(p => p.Name == fullName);
            if (bit != null)
            {
                Type = TypeReferenceType.Integer;
                IntegerType = bit;
                return;
            }
            else if (ArrayType.ArrayTypeRegex.IsMatch(fullName))
            {
                Type = TypeReferenceType.Array;
                ArrayType = new ArrayType(fullName, assembly);
            }
            else if(fullName == StringType.StringTypeKeyword)
            {
                Type = TypeReferenceType.Array;
                ArrayType = new StringType(assembly);
            } else
            {
                Type = TypeReferenceType.Class;
            }
            assembly?.RegisterType(this);
        }

        public static bool operator ==(TypeReference a, TypeReference b)
        {
			if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b,null))
				return true;

			if ((Object.ReferenceEquals(a, null) && !(Object.ReferenceEquals(b, null))) ||
				(!(Object.ReferenceEquals(a, null)) && Object.ReferenceEquals(b, null)))
				return false;

            if (a.Registered && b.Registered)
                return a.UniqueID == b.UniqueID;

            return a.Name == b.Name;
        }

        public static bool operator !=(TypeReference a, TypeReference b)
        {
            return !(a == b);
        }
    }
}
