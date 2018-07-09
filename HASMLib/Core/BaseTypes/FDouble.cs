using System;

namespace HASMLib.Core.BaseTypes
{
    public struct FDouble
    {
        internal UInt64 Value;

        internal FDouble(ulong value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator >(FDouble a, FDouble b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(FDouble a, FDouble b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(FDouble a, FDouble b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(FDouble a, FDouble b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator byte(FDouble a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(FDouble a)
        {
            return (UInt16)(a.Value & 0xFFFF);
        }

        public static implicit operator UInt32(FDouble a)
        {
            return (UInt32)(a.Value & 0xFFFFFF);
        }

        public static implicit operator UInt64(FDouble a)
        {
            return (UInt64)(a.Value & 0xFFFFFFFF);
        }

        public static implicit operator FDouble(byte a)
        {
            return new FDouble(a & HASMBase.DoubleBitMask);
        }

        public static implicit operator FDouble(UInt16 a)
        {
            return new FDouble(a & HASMBase.DoubleBitMask);
        }

        public static implicit operator FDouble(UInt32 a)
        {
            return new FDouble(a & HASMBase.DoubleBitMask);
        }

        public static implicit operator FDouble(UInt64 a)
        {
            return new FDouble(a & HASMBase.DoubleBitMask);
        }

        public static implicit operator FDouble(Int16 a)
        {
            return new FDouble((ushort)a & HASMBase.DoubleBitMask);
        }

        public static implicit operator FDouble(Int32 a)
        {
            return new FDouble((uint)a & HASMBase.DoubleBitMask);
        }

        public static implicit operator FDouble(Int64 a)
        {
            return new FDouble((ulong)a & HASMBase.DoubleBitMask);
        }

        public static implicit operator FDouble(FSingle a)
        {
            return new FDouble(a);
        }

        public static implicit operator FSingle(FDouble a)
        {
            return new FSingle(a.Value & HASMBase.SingleBitMask);
        }

        public static FDouble operator +(FDouble a, FDouble b)
        {
            return new FDouble((a.Value + b.Value) & HASMBase.DoubleBitMask);
        }

        public static FDouble operator -(FDouble a, FDouble b)
        {
            return new FDouble((a.Value - b.Value) & HASMBase.DoubleBitMask);
        }

        public static FDouble operator *(FDouble a, FDouble b)
        {
            return new FDouble((a.Value * b.Value ) & HASMBase.DoubleBitMask);
        }

        public static FDouble operator /(FDouble a, FDouble b)
        {
            return new FDouble((a.Value / b.Value) & HASMBase.DoubleBitMask);
        }

        public FSingle[] ToSingle()
        {
            return new FSingle[2]
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

        public static FDouble FromSingle(FSingle[] data)
        {
            if (data.Length != 2)
                throw new ArgumentException("Length must be 2!");

            return new FDouble() { Value = (((uint)data[1] << (int)HASMBase.Base) | data[0]) & HASMBase.DoubleBitMask };
        }

        public override bool Equals(object obj)
        {
            return (GetType() == obj.GetType()) && (Value == ((FDouble)obj).Value);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}