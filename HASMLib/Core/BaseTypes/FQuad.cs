using System;

namespace HASMLib.Core.BaseTypes
{
    public struct FQuad
    {
        private ulong Value;

        private FQuad(ulong value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator >(FQuad a, FQuad b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(FQuad a, FQuad b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(FQuad a, FQuad b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(FQuad a, FQuad b)
        {
            return a.Value != b.Value;
        }

        public static implicit operator byte(FQuad a)
        {
            return (byte)(a.Value & 0xFF);
        }

        public static implicit operator UInt16(FQuad a)
        {
            return (UInt16)(a.Value & 0xFFFF);
        }

        public static implicit operator UInt32(FQuad a)
        {
            return (UInt32)(a.Value & 0xFFFFFF);
        }

        public static implicit operator UInt64(FQuad a)
        {
            return (UInt64)(a.Value & 0xFFFFFFFF);
        }

        public static implicit operator FQuad(byte a)
        {
            return new FQuad(a & HASMBase.QuadBitMask);
        }

        public static implicit operator FQuad(UInt16 a)
        {
            return new FQuad(a & HASMBase.QuadBitMask);
        }

        public static implicit operator FQuad(UInt32 a)
        {
            return new FQuad(a & HASMBase.QuadBitMask);
        }

        public static implicit operator FQuad(UInt64 a)
        {
            return new FQuad(a & HASMBase.QuadBitMask);
        }

        public static implicit operator FQuad(Int16 a)
        {
            return new FQuad((ushort)a & HASMBase.QuadBitMask);
        }

        public static implicit operator FQuad(Int32 a)
        {
            return new FQuad((uint)a & HASMBase.QuadBitMask);
        }

        public static implicit operator FQuad(Int64 a)
        {
            return new FQuad((ulong)a & HASMBase.QuadBitMask);
        }

        public static implicit operator FQuad(FSingle a)
        {
            return new FQuad(a);
        }

        public static implicit operator FSingle(FQuad a)
        {
            return new FSingle(a.Value & HASMBase.SingleBitMask);
        }

        public static implicit operator FQuad(FDouble a)
        {
            return new FQuad(a);
        }

        public static implicit operator FDouble(FQuad a)
        {
            return new FDouble(a.Value & HASMBase.DoubleBitMask);
        }

        public static FQuad operator +(FQuad a, FQuad b)
        {
            return new FQuad((a.Value + b.Value) & HASMBase.QuadBitMask);
        }

        public static FQuad operator -(FQuad a, FQuad b)
        {
            return new FQuad((a.Value - b.Value) & HASMBase.QuadBitMask);
        }

        public static FQuad operator *(FQuad a, FQuad b)
        {
            return new FQuad((a.Value * b.Value) & HASMBase.QuadBitMask);
        }

        public static FQuad operator /(FQuad a, FQuad b)
        {
            return new FQuad((a.Value / b.Value) & HASMBase.QuadBitMask);
        }

        public FSingle[] ToSingle()
        {
            return new FSingle[4]
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

        public static FQuad FromSingle(FSingle[] data)
        {
            if (data.Length != 4)
                throw new ArgumentException("Length must be 4!");

            return new FQuad()
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
            return (GetType() == obj.GetType()) && (Value == ((FQuad)obj).Value);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}