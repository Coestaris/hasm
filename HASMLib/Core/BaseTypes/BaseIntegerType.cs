using System;
using System.Collections.Generic;

namespace HASMLib.Core.BaseTypes
{
    public class BaseIntegerType
    {
        public static List<BaseIntegerType> Types;
        public static BaseIntegerType PrimitiveType;
        public static BaseIntegerType CommonType;
        public static BaseIntegerType CommonSignedType;
        public static BaseIntegerType CommonCharType;

        public int UniqueID;
        public int Base;
        public long MinValue;
        public ulong MaxValue;
        public ulong BitMask;
        public bool IsSigned;
        public string Name;

        public static bool operator ==(BaseIntegerType a, BaseIntegerType b)
        {
			if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b,null))
				return true;

			if ((Object.ReferenceEquals(a, null) && !(Object.ReferenceEquals(b, null))) ||
				(!(Object.ReferenceEquals(a, null)) && Object.ReferenceEquals(b, null)))
				return false;

            return a.Equals(b);
        }

        public static bool operator !=(BaseIntegerType a, BaseIntegerType b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
			return obj is BaseIntegerType && 
				(obj as BaseIntegerType).Base == Base && 
				(obj as BaseIntegerType).IsSigned == IsSigned;
        }

        public BaseIntegerType(int _base, bool isSigned, string name)
        {
            Base = _base;
            if (_base >= 64 && !isSigned)
            {
                MaxValue = ulong.MaxValue;
            }
            else
            {
                MaxValue = (ulong)(Math.Pow(2, _base - (isSigned ? 1 : 0)) - 1);
            }

            Name = name;
            MinValue = isSigned ? (-(long)MaxValue + 1) : 0;
            BitMask = (ulong)Math.Pow(2, _base) - 1;
        }


        internal List<Integer> Cast(Integer integer)
        {
            return integer.Cast(this);
        }

        public override string ToString()
        {
            return $"{Name} - ({Base} bits)";
        }

        public override int GetHashCode()
        {
            var hashCode = -1036935733;
            hashCode = hashCode * -1521134295 + UniqueID.GetHashCode();
            hashCode = hashCode * -1521134295 + Base.GetHashCode();
            hashCode = hashCode * -1521134295 + IsSigned.GetHashCode();
            return hashCode;
        }
    }
}
