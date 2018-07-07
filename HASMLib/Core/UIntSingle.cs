using System;

namespace HASMLib.Core
{
    public struct UIntSingle
    {
        internal ulong Value;

        internal UIntSingle(ulong value)
        {
            Value = value;
        }

        public string ToString(string format)
        {
            return Value.ToString(format);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator >(UIntSingle a, UIntSingle b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(UIntSingle a, UIntSingle b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(UIntSingle a, UIntSingle b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(UIntSingle a, UIntSingle b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator char(UIntSingle a)
        {
            return (char)((UInt16)a);
        }

        public static implicit operator byte(UIntSingle a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(UIntSingle a)
        {
            return (UInt16)(a.Value & 0xFFFF);
        }

        public static implicit operator UInt32(UIntSingle a)
        {
            return (UInt32)(a.Value & 0xFFFFFF);
        }

        public static implicit operator UInt64(UIntSingle a)
        {
            return (UInt64)(a.Value & 0xFFFFFFFF);
        }

        public static implicit operator UIntSingle(byte a)
        {
            return new UIntSingle(a & HASMBase.SingleBitMask);
        }

        public static implicit operator UIntSingle(UInt16 a)
        {
            return new UIntSingle(a & HASMBase.SingleBitMask);
        }

        public static implicit operator UIntSingle(UInt32 a)
        {
            return new UIntSingle(a & HASMBase.SingleBitMask);
        }

        public static implicit operator UIntSingle(UInt64 a)
        {
            return new UIntSingle(a & HASMBase.SingleBitMask);
        }

        public static implicit operator UIntSingle(Int16 a)
        {
            return new UIntSingle((ushort)a & HASMBase.SingleBitMask);
        }

        public static implicit operator UIntSingle(Int32 a)
        {
            return new UIntSingle((uint)a & HASMBase.SingleBitMask);
        }

        public static implicit operator UIntSingle(Int64 a)
        {
            return new UIntSingle((ulong)a & HASMBase.SingleBitMask);
        }

        public static UIntSingle operator +(UIntSingle a, UIntSingle b)
        {
            return new UIntSingle((a.Value + b.Value) & HASMBase.SingleBitMask);
        }

        public static UIntSingle operator -(UIntSingle a, UIntSingle b)
        {
            return new UIntSingle((a.Value - b.Value) & HASMBase.SingleBitMask);
        }

        public static UIntSingle operator *(UIntSingle a, UIntSingle b)
        {
            return new UIntSingle((a.Value * b.Value) & HASMBase.SingleBitMask);
        }

        public static UIntSingle operator /(UIntSingle a, UIntSingle b)
        {
            return new UIntSingle((a.Value / b.Value) & HASMBase.SingleBitMask);
        }

		public byte[] ToBytes()
		{
			return new byte[2] {
				(byte)((Value) & 0xFF),
				(byte)((Value >> 8) & 0xFF),
			};
		}

        public override bool Equals(object obj)
        {
            return (GetType() == obj.GetType()) && (Value == ((UIntSingle)obj).Value);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}