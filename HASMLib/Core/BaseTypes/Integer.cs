using System;
using System.Collections.Generic;

namespace HASMLib.Core.BaseTypes
{
    public struct Integer
    {
        public BaseIntegerType Type;
        public ulong Value;

        public override string ToString()
        {
            return $"{Value}({Type.Name})";
        }

        public string ToString(string format)
        {
            return $"{Value.ToString(format)}({Type.Name})";
        }

        internal static BaseIntegerType SelectType(Integer a, Integer b)
        {
            if (a.Type.Base >= b.Type.Base)
                return a.Type;
            else return b.Type;
        }

        public Integer(ulong value, BaseIntegerType type)
        {
            unchecked
            {
                Type = type;
                Value = value & type.BitMask;
            }
        }

        public static Integer operator ~(Integer a)
        {
            return unchecked(new Integer(~a.Value, a.Type));
        }
        
        public static bool operator >=(Integer a, Integer b)
        {
            return unchecked(a.Value >= b.Value);
        }

        public static bool operator <=(Integer a, Integer b)
        {
            return unchecked(a.Value <= b.Value);
        }

        public static bool operator >(Integer a, Integer b)
        {
            return unchecked(a.Value > b.Value);
        }

        public static bool operator <(Integer a, Integer b)
        {
            return unchecked(a.Value < b.Value);
        }

        public static bool operator ==(Integer a, Integer b)
        {
            return unchecked(a.Value == b.Value);
        }

        public static bool operator !=(Integer a, Integer b)
        {
            return unchecked(a.Value != b.Value);
        }

        public static explicit operator byte(Integer a)
        {
            return unchecked((byte)(a.Value & 0xFF));
        }

        public static explicit operator UInt16(Integer a)
        {
            return unchecked((UInt16)(a.Value & 0xFFFF));
        }

        public static explicit operator UInt32(Integer a)
        {
            return unchecked((UInt32)(a.Value & 0xFFFFFF));
        }

        public byte[] ToBytes()
        {
            unchecked
            {
                if (Type.Base < 8)
                    return new byte[] { (byte)Value };

                int count = Type.Base / 8;

                var result = new List<byte>();
                for (int i = 0; i < count; i++)
                    result.Add((byte)((Value >> i * 8) & 0xFF));

                return result.ToArray();
            }
        }

        public static explicit operator Integer(UInt64 a)
        {
            return unchecked(new Integer((ulong)a, BaseIntegerType.CommonType));
        }

        public static explicit operator Integer(UInt32 a)
        {
            return unchecked(new Integer(a, BaseIntegerType.CommonType));
        }

        public static explicit operator Integer(UInt16 a)
        {
            return unchecked(new Integer(a, BaseIntegerType.CommonType));
        }

        public static explicit operator Integer(byte a)
        {
            return unchecked(new Integer(a, BaseIntegerType.CommonType));
        }

        public static explicit operator Integer(Int64 a)
        {
            return unchecked(new Integer((ulong)a, BaseIntegerType.CommonType));
        }

        public static explicit operator Integer(Int32 a)
        {
            return unchecked(new Integer((ulong)a, BaseIntegerType.CommonType));
        }

        public static explicit operator Integer(Int16 a)
        {
            return unchecked(new Integer((ulong)a, BaseIntegerType.CommonType));
        }

        public static explicit operator Integer(sbyte a)
        {
            return unchecked(new Integer((ulong)a, BaseIntegerType.CommonType));
        }

        public static explicit operator UInt64(Integer a)
        {
            return unchecked((UInt64)(a.Value & 0xFFFFFFFF));
        }

        public static explicit operator Int16(Integer a)
        {
            return unchecked((Int16)(a.Value & 0xFFFF));
        }

        public static explicit operator Int32(Integer a)
        {
            return unchecked((Int32)(a.Value & 0xFFFFFF));
        }

        public static explicit operator Int64(Integer a)
        {
            return unchecked((Int64)(a.Value & 0xFFFFFFFF));
        }

        public static Integer operator <<(Integer a, int b)
        {
            return unchecked(new Integer(a.Value << b, a.Type));
        }

        public static Integer operator %(Integer a, Integer b)
        {
            return unchecked(new Integer(a.Value % b.Value, SelectType(a, b)));
        }

        public static Integer operator >>(Integer a, int b)
        {
            return unchecked(new Integer(a.Value >> b, a.Type));
        }

        public static Integer operator &(Integer a, Integer b)
        {
            return unchecked(new Integer(a.Value & b.Value, SelectType(a, b)));
        }

        public static Integer operator |(Integer a, Integer b)
        {
            return unchecked(new Integer(a.Value | b.Value, SelectType(a, b)));
        }

        public static Integer operator ^(Integer a, Integer b)
        {
            return unchecked(new Integer(a.Value ^ b.Value, SelectType(a, b)));
        }

        public static Integer operator +(Integer a, Integer b)
        {
            return unchecked(new Integer(a.Value + b.Value, SelectType(a, b)));
        }

        public static Integer operator -(Integer a, Integer b)
        {
            return unchecked(new Integer(a.Value - b.Value, SelectType(a, b)));
        }

        public static Integer operator *(Integer a, Integer b)
        {
            return unchecked(new Integer(a.Value * b.Value, SelectType(a, b)));
        }

        public static Integer operator /(Integer a, Integer b)
        {
            return unchecked(new Integer(a.Value / b.Value, SelectType(a, b)));
        }

        public List<Integer> Cast(BaseIntegerType type)
        {
            unchecked
            {
                if (Type.Base < type.Base)
                    return new List<Integer>() { new Integer(Value, type) };

                int count = Type.Base / type.Base;
                var result = new List<Integer>();
                for (int i = 0; i < count; i++)
                    result.Add(new Integer(Value >> i * type.Base, type));

                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Integer))
            {
                return false;
            }

            var integer = (Integer)obj;
            return Type.Base == integer.Type.Base &&
                   Value == integer.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = 1265339359;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseIntegerType>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }
    }
}
