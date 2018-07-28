﻿using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures.Units;

namespace HASMLib.Runtime.Structures
{
    public class TypeReference
    {
        public int UinqueID 
        {
            get
            {
                if (IsVoid) return 0;
                if (IsBaseInteger) return IntegerType.UniqueID;
                else return ClassType.UniqueID;
            }
        }

        public string Name;

        public bool IsBaseInteger;
        public BaseIntegerType IntegerType;

        public bool IsClass;
        public Class ClassType;

        public bool IsVoid;

        public static TypeReference Void => new TypeReference();

        public override string ToString()
        {
            if (IsVoid) return Function.NoReturnableValueKeyword;

            if (!IsClass && !IsBaseInteger) return $"Unknown reference to: {Name}";

            return IsBaseInteger ? IntegerType.Name : ClassType.FullName;
        }

        private TypeReference()
        {
            IsVoid = true;
        }

        public TypeReference(BaseIntegerType intType)
        {
            Name = intType.Name;
            IsBaseInteger = true;
            IntegerType = intType;
        }

        public TypeReference(Class Class)
        {
            IsClass = true;
            Name = Class.FullName;
            ClassType = Class;
        }

        public TypeReference(string fullName)
        {
            Name = fullName;
            var bit = BaseIntegerType.Types.Find(p => p.Name == fullName);
            if(bit != null)
            {
                IsBaseInteger = true;
                IntegerType = bit;
                return;
            }
        }
    }
}
