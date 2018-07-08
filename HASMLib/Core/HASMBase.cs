using System;

namespace HASMLib.Core
{
    public static class HASMBase
    {
        private static uint _base;

        private static ulong singleBase;
        private static ulong doubleBase;
        private static ulong quadBase;

        private static ulong singleMaxValue;
        private static ulong doubleMaxValue;
        private static ulong quadMaxValue;

        private static ulong singleBitMask;
        private static ulong doubleBitMask;
        private static ulong quadBitMask;

        private static bool _set = false;

        public static uint Base
        {
            get => _base;
            set
            {
                if (value > 16)
                    throw new ArgumentException("Пока битность не может превышать 16");

                if(value <= 0)
                    throw new ArgumentException("Битность должна быть больше 0");


                _set = true;

                _base = value;
                singleBase = _base;
                doubleBase = _base * 2u;
                quadBase = _base * 4u;

                singleMaxValue = (ulong)Math.Pow(2, singleBase);
                doubleMaxValue = (ulong)Math.Pow(2, doubleBase);
                quadMaxValue = (ulong)Math.Pow(2, quadBase);

                singleBitMask = singleMaxValue - 1;
                doubleBitMask = doubleMaxValue - 1;
                quadBitMask = quadMaxValue - 1;
            }
        }

        internal static ulong SingleBase        { get => !_set ? throw new InvalidOperationException() : singleBase; }
        internal static ulong DoubleBase        { get => !_set ? throw new InvalidOperationException() : doubleBase; }
        internal static ulong QuadBase          { get => !_set ? throw new InvalidOperationException() : quadBase; }
        internal static ulong SingleMaxValue    { get => !_set ? throw new InvalidOperationException() : singleMaxValue; }
        internal static ulong DoubleMaxValue    { get => !_set ? throw new InvalidOperationException() : doubleMaxValue; }
        internal static ulong QuadMaxValue      { get => !_set ? throw new InvalidOperationException() : quadMaxValue; }
        internal static ulong SingleBitMask     { get => !_set ? throw new InvalidOperationException() : singleBitMask; }
        internal static ulong DoubleBitMask     { get => !_set ? throw new InvalidOperationException() : doubleBitMask; }
        internal static ulong QuadBitMask       { get => !_set ? throw new InvalidOperationException() : quadBitMask; }
    }
}
