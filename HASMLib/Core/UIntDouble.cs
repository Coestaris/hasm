using System;

namespace HASMLib.Core
{
    public struct UIntDouble
    {
        internal UInt64 Value;

        internal UIntDouble(ulong value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator >(UIntDouble a, UIntDouble b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(UIntDouble a, UIntDouble b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(UIntDouble a, UIntDouble b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(UIntDouble a, UIntDouble b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator byte(UIntDouble a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(UIntDouble a)
        {
            return (UInt16)(a.Value & 0xFFFF);
        }

        public static implicit operator UInt32(UIntDouble a)
        {
            return (UInt32)(a.Value & 0xFFFFFF);
        }

        public static implicit operator UInt64(UIntDouble a)
        {
            return (UInt64)(a.Value & 0xFFFFFFFF);
        }

        public static implicit operator UIntDouble(byte a)
        {
            return new UIntDouble(a & HASMBase.DoubleBitMask);
        }

        public static implicit operator UIntDouble(UInt16 a)
        {
            return new UIntDouble(a & HASMBase.DoubleBitMask);
        }

        public static implicit operator UIntDouble(UInt32 a)
        {
            return new UIntDouble(a & HASMBase.DoubleBitMask);
        }

        public static implicit operator UIntDouble(UInt64 a)
        {
            return new UIntDouble(a & HASMBase.DoubleBitMask);
        }

        public static implicit operator UIntDouble(Int16 a)
        {
            return new UIntDouble((ushort)a & HASMBase.DoubleBitMask);
        }

        public static implicit operator UIntDouble(Int32 a)
        {
            return new UIntDouble((uint)a & HASMBase.DoubleBitMask);
        }

        public static implicit operator UIntDouble(Int64 a)
        {
            return new UIntDouble((ulong)a & HASMBase.DoubleBitMask);
        }

        public static implicit operator UIntDouble(UIntSingle a)
        {
            return new UIntDouble(a);
        }

        public static implicit operator UIntSingle(UIntDouble a)
        {
            return new UIntSingle(a.Value & HASMBase.SingleBitMask);
        }

        public static UIntDouble operator +(UIntDouble a, UIntDouble b)
        {
            return new UIntDouble((a.Value + b.Value) & HASMBase.DoubleBitMask);
        }

        public static UIntDouble operator -(UIntDouble a, UIntDouble b)
        {
            return new UIntDouble((a.Value - b.Value) & HASMBase.DoubleBitMask);
        }

        public static UIntDouble operator *(UIntDouble a, UIntDouble b)
        {
            return new UIntDouble((a.Value * b.Value ) & HASMBase.DoubleBitMask);
        }

        public static UIntDouble operator /(UIntDouble a, UIntDouble b)
        {
            return new UIntDouble((a.Value / b.Value) & HASMBase.DoubleBitMask);
        }

        public UIntSingle[] ToUInt12()
        {
            return new UIntSingle[2]
            {
                (Value & HASMBase.SingleBitMask),
                ((Value >> (int)HASMBase.Base) & HASMBase.SingleBitMask)
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

        public static UIntDouble FromUInt12(UIntSingle[] data)
        {
            if (data.Length != 2)
                throw new ArgumentException("Length must be 2!");

            return new UIntDouble() { Value = (((uint)data[1] << (int)HASMBase.Base) | data[0]) & HASMBase.DoubleBitMask };
        }

        public override bool Equals(object obj)
        {
            return (GetType() == obj.GetType()) && (Value == ((UIntDouble)obj).Value);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}