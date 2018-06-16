using System;

namespace HASMLib.Core
{
    public struct UInt24
    {
        internal UInt32 Value;

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator >(UInt24 a, UInt24 b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(UInt24 a, UInt24 b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(UInt24 a, UInt24 b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(UInt24 a, UInt24 b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator byte(UInt24 a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(UInt24 a)
        {
            return (UInt16)(a.Value & 0xFFFF);
        }

        public static implicit operator UInt32(UInt24 a)
        {
            return a.Value;
        }

        public static implicit operator UInt64(UInt24 a)
        {
            return a.Value;
        }

        public static implicit operator UInt24(byte a)
        {
            return new UInt24() { Value = a };
        }

        public static implicit operator UInt24(UInt16 a)
        {
            return new UInt24() { Value = a };
        }

        public static implicit operator UInt24(UInt32 a)
        {
            return new UInt24() { Value = (UInt32)(a & 0xFFFFF) };
        }

        public static implicit operator UInt24(UInt64 a)
        {
            return new UInt24() { Value = (UInt32)(a & 0xFFFFF) };
        }

        public static implicit operator UInt24(UInt12 a)
        {
            return new UInt24() { Value = a };
        }

        public static implicit operator UInt12(UInt24 a)
        {
            return new UInt12() { Value = (UInt16)(a.Value & 0xFFF) };
        }

        public static UInt24 operator +(UInt24 a, UInt24 b)
        {
            return new UInt24() { Value = (UInt32)(a.Value + b.Value) };
        }

        public static UInt24 operator -(UInt24 a, UInt24 b)
        {
            return new UInt24() { Value = (UInt32)(a.Value - b.Value) };
        }

        public static UInt24 operator *(UInt24 a, UInt24 b)
        {
            return new UInt24() { Value = (UInt32)(a.Value * b.Value) };
        }

        public static UInt24 operator /(UInt24 a, UInt24 b)
        {
            return new UInt24() { Value = (UInt32)(a.Value / b.Value) };
        }

        public UInt12[] ToUInt12()
        {
            return new UInt12[2]
            {
                (Value & 0xFFF),
                ((Value >> 12) & 0xFFF)
            };
        }

		public byte[] ToBytes()
		{
			return new byte[3] {
				(byte)((Value) & 0xFF),
				(byte)((Value >> 8) & 0xFF),
				(byte)((Value >> 16) & 0xFF),
			};
		}

        public static UInt24 FromUInt12(UInt12[] data)
        {
            if (data.Length != 2)
                throw new ArgumentException("Length must be 2!");

            return new UInt24() { Value = (((uint)data[1] << 12) | data[0]) & 0xFFFFFF };
        }

        public override bool Equals(object obj)
        {
            return (GetType() == obj.GetType()) && (Value == ((UInt24)obj).Value);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}