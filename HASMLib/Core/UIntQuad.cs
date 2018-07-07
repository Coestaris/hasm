using System;

namespace HASMLib.Core
{
    public struct UIntQuad
    {
        private ulong Value;

        private UIntQuad(ulong value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator >(UIntQuad a, UIntQuad b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(UIntQuad a, UIntQuad b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(UIntQuad a, UIntQuad b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(UIntQuad a, UIntQuad b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator byte(UIntQuad a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(UIntQuad a)
        {
            return (UInt16)(a.Value & 0xFFFF);
        }

        public static implicit operator UInt32(UIntQuad a)
        {
            return (UInt32)(a.Value & 0xFFFFFF);
        }

        public static implicit operator UInt64(UIntQuad a)
        {
            return (UInt64)(a.Value & 0xFFFFFFFF);
        }

        public static implicit operator UIntQuad(byte a)
        {
            return new UIntQuad(a & HASMBase.QuadBitMask);
        }

        public static implicit operator UIntQuad(UInt16 a)
        {
            return new UIntQuad(a & HASMBase.QuadBitMask);
        }

        public static implicit operator UIntQuad(UInt32 a)
        {
            return new UIntQuad(a & HASMBase.QuadBitMask);
        }

        public static implicit operator UIntQuad(UInt64 a)
        {
            return new UIntQuad(a & HASMBase.QuadBitMask);
        }

        public static implicit operator UIntQuad(Int16 a)
        {
            return new UIntQuad((ushort)a & HASMBase.QuadBitMask);
        }

        public static implicit operator UIntQuad(Int32 a)
        {
            return new UIntQuad((uint)a & HASMBase.QuadBitMask);
        }

        public static implicit operator UIntQuad(Int64 a)
        {
            return new UIntQuad((ulong)a & HASMBase.QuadBitMask);
        }

        public static implicit operator UIntQuad(UIntSingle a)
        {
            return new UIntQuad(a);
        }

        public static implicit operator UIntSingle(UIntQuad a)
        {
            return new UIntSingle(a.Value & HASMBase.SingleBitMask);
        }

        public static implicit operator UIntQuad(UIntDouble a)
        {
            return new UIntQuad(a);
        }

        public static implicit operator UIntDouble(UIntQuad a)
        {
            return new UIntDouble(a.Value & HASMBase.DoubleBitMask);
        }

        public static UIntQuad operator +(UIntQuad a, UIntQuad b)
        {
            return new UIntQuad((a.Value + b.Value) & HASMBase.QuadBitMask);
        }

        public static UIntQuad operator -(UIntQuad a, UIntQuad b)
        {
            return new UIntQuad((a.Value - b.Value) & HASMBase.QuadBitMask);
        }

        public static UIntQuad operator *(UIntQuad a, UIntQuad b)
        {
            return new UIntQuad((a.Value * b.Value) & HASMBase.QuadBitMask);
        }

        public static UIntQuad operator /(UIntQuad a, UIntQuad b)
        {
            return new UIntQuad((a.Value / b.Value) & HASMBase.QuadBitMask);
        }

        public UIntSingle[] ToUInt12()
        {
            return new UIntSingle[4]
            {
                Value & HASMBase.SingleBitMask,
                (Value >> (int)HASMBase.Base)  & HASMBase.SingleBitMask,
                (Value >> (int)HASMBase.Base * 2) & HASMBase.SingleBitMask,
                (Value >> (int)HASMBase.Base * 3) & HASMBase.SingleBitMask
            };
        }

        public byte[] ToBytes()
        {
            return new byte[5] {
                (byte)((Value) & 0xFF),
                (byte)((Value >> 8) & 0xFF),
                (byte)((Value >> 16) & 0xFF),
                (byte)((Value >> 24) & 0xFF),
                (byte)((Value >> 32) & 0xFF),
            };
        }

        public static UIntQuad FromUInt12(UIntSingle[] data)
        {
            if (data.Length != 4)
                throw new ArgumentException("Length must be 4!");

            return new UIntQuad()
            {
                Value = 
                    (((UInt64)data[3] << (int)HASMBase.Base * 3) |
                     ((UInt64)data[2] << (int)HASMBase.Base * 2) |
                     ((UInt64)data[1] << (int)HASMBase.Base)     | 
                              data[0]) & HASMBase.QuadBitMask
            };
        }

        public override bool Equals(object obj)
        {
            return (GetType() == obj.GetType()) && (Value == ((UIntQuad)obj).Value);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}