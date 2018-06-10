using System;

namespace HASMLib.Core
{
    public struct UInt12
    {
        internal UInt16 Value;

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator >(UInt12 a, UInt12 b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(UInt12 a, UInt12 b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(UInt12 a, UInt12 b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(UInt12 a, UInt12 b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator byte(UInt12 a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(UInt12 a)
        {
            return a.Value;
        }

        public static implicit operator UInt32(UInt12 a)
        {
            return a.Value;
        }

        public static implicit operator UInt64(UInt12 a)
        {
            return a.Value;
        }

        public static implicit operator UInt12(byte a)
        {
            return new UInt12() { Value = a };
        }

        public static implicit operator UInt12(UInt16 a)
        {
            return new UInt12() { Value = (UInt16)(a & 0xFFF) };
        }

        public static implicit operator UInt12(UInt32 a)
        {
            return new UInt12() { Value = (UInt16)(a & 0xFFF) };
        }

        public static implicit operator UInt12(UInt64 a)
        {
            return new UInt12() { Value = (UInt16)(a & 0xFFF) };
        }

        public static UInt12 operator +(UInt12 a, UInt12 b)
        {
            return new UInt12() { Value = (UInt16)(a.Value + b.Value) };
        }

        public static UInt12 operator -(UInt12 a, UInt12 b)
        {
            return new UInt12() { Value = (UInt16)(a.Value - b.Value) };
        }

        public static UInt12 operator *(UInt12 a, UInt12 b)
        {
            return new UInt12() { Value = (UInt16)(a.Value * b.Value) };
        }

        public static UInt12 operator /(UInt12 a, UInt12 b)
        {
            return new UInt12() { Value = (UInt16)(a.Value / b.Value) };
        }
    }
}