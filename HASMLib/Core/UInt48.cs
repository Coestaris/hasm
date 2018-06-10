using System;

namespace HASMLib.Core
{
    public struct UInt48
    {
        private UInt64 Value;

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator >(UInt48 a, UInt48 b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(UInt48 a, UInt48 b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(UInt48 a, UInt48 b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(UInt48 a, UInt48 b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator byte(UInt48 a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(UInt48 a)
        {
            return (UInt16)(a.Value & 0xFFFF);
        }

        public static implicit operator UInt32(UInt48 a)
        {
            return (UInt32)(a.Value & 0xFFFFFF);
        }

        public static implicit operator UInt64(UInt48 a)
        {
            return a.Value;
        }

        public static implicit operator UInt48(byte a)
        {
            return new UInt48() { Value = a };
        }

        public static implicit operator UInt48(UInt16 a)
        {
            return new UInt48() { Value = a };
        }

        public static implicit operator UInt48(UInt32 a)
        {
            return new UInt48() { Value = a };
        }

        public static implicit operator UInt48(UInt64 a)
        {
            return new UInt48() { Value = (UInt64)(a & 0xFFFFFFFFFFFF) };
        }

        public static implicit operator UInt48(UInt12 a)
        {
            return new UInt48() { Value = a };
        }

        public static implicit operator UInt12(UInt48 a)
        {
            return new UInt12() { Value = (UInt16)(a.Value & 0xFFF) };
        }

        public static implicit operator UInt48(UInt24 a)
        {
            return new UInt48() { Value = a };
        }

        public static implicit operator UInt24(UInt48 a)
        {
            return new UInt24() { Value = (UInt32)(a.Value & 0xFFFFF) };
        }

        public static UInt48 operator +(UInt48 a, UInt48 b)
        {
            return new UInt48() { Value = (UInt64)(a.Value + b.Value) };
        }

        public static UInt48 operator -(UInt48 a, UInt48 b)
        {
            return new UInt48() { Value = (UInt64)(a.Value - b.Value) };
        }

        public static UInt48 operator *(UInt48 a, UInt48 b)
        {
            return new UInt48() { Value = (UInt64)(a.Value * b.Value) };
        }

        public static UInt48 operator /(UInt48 a, UInt48 b)
        {
            return new UInt48() { Value = (UInt64)(a.Value / b.Value) };
        }

        public UInt12[] ToUInt12()
        {
            return new UInt12[4]
            {
                (Value),
                ((Value >> 12)),
                ((Value >> 24)),
                ((Value >> 36))
            };
        }

        public static UInt48 FromUInt12(UInt12[] data)
        {
            if (data.Length != 4)
                throw new ArgumentException("Length must be 4!");

            return new UInt48()
            {
                Value = 
                    (((UInt64)data[3] << 36) |
                     ((UInt64)data[2] << 24) |
                     ((UInt64)data[1] << 12) | 
                              data[0]         ) & 0xFFFFFFFFFFFFUL
            };
        }
    }
}