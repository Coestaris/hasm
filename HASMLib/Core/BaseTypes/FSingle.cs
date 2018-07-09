using System;

namespace HASMLib.Core.BaseTypes
{
    public struct FSingle
    {
        internal ulong Value;

        internal FSingle(ulong value)
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

        public static bool operator >(FSingle a, FSingle b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(FSingle a, FSingle b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(FSingle a, FSingle b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(FSingle a, FSingle b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator char(FSingle a)
        {
            return (char)((UInt16)a);
        }

        public static implicit operator byte(FSingle a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(FSingle a)
        {
            return (UInt16)(a.Value & 0xFFFF);
        }

        public static implicit operator UInt32(FSingle a)
        {
            return (UInt32)(a.Value & 0xFFFFFF);
        }

        public static implicit operator UInt64(FSingle a)
        {
            return (UInt64)(a.Value & 0xFFFFFFFF);
        }

        public static implicit operator FSingle(byte a)
        {
            return new FSingle(a & HASMBase.SingleBitMask);
        }

        public static implicit operator FSingle(UInt16 a)
        {
            return new FSingle(a & HASMBase.SingleBitMask);
        }

        public static implicit operator FSingle(UInt32 a)
        {
            return new FSingle(a & HASMBase.SingleBitMask);
        }

        public static implicit operator FSingle(UInt64 a)
        {
            return new FSingle(a & HASMBase.SingleBitMask);
        }

        public static implicit operator FSingle(Int16 a)
        {
            return new FSingle((ushort)a & HASMBase.SingleBitMask);
        }

        public static implicit operator FSingle(Int32 a)
        {
            return new FSingle((uint)a & HASMBase.SingleBitMask);
        }

        public static implicit operator FSingle(Int64 a)
        {
            return new FSingle((ulong)a & HASMBase.SingleBitMask);
        }

        public static FSingle operator +(FSingle a, FSingle b)
        {
            return new FSingle((a.Value + b.Value) & HASMBase.SingleBitMask);
        }

        public static FSingle operator -(FSingle a, FSingle b)
        {
            return new FSingle((a.Value - b.Value) & HASMBase.SingleBitMask);
        }

        public static FSingle operator *(FSingle a, FSingle b)
        {
            return new FSingle((a.Value * b.Value) & HASMBase.SingleBitMask);
        }

        public static FSingle operator /(FSingle a, FSingle b)
        {
            return new FSingle((a.Value / b.Value) & HASMBase.SingleBitMask);
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
            return (GetType() == obj.GetType()) && (Value == ((FSingle)obj).Value);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}