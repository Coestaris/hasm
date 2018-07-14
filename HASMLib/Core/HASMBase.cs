using HASMLib.Core.BaseTypes;
using System;
using System.Collections.Generic;

namespace HASMLib.Core
{
    public static class HASMBase
    {
        private static int _base;

        private static bool _set = false;

        public static bool IsSTD => _base == 64 || _base == 32 || _base == 16 || _base == 8;

        public static int PrimitiveTypesInCommon => BaseIntegerType.CommonType.Base / BaseIntegerType.PrimitiveType.Base;

        public static int Base
        {
            get => _base;
            set
            {
                _base = value;
                _set = true;
                switch (value)
                {
                    case (64):
                    case (32):
                    case (16):
                    case (8):
                        BaseIntegerType.Types = new List<BaseIntegerType>()
                        {
                            new BaseIntegerType(64, false, "ulong"),
                            new BaseIntegerType(64, true , "long"),
                            new BaseIntegerType(32, false, "uint"),
                            new BaseIntegerType(32, true , "int"),
                            new BaseIntegerType(16, false, "ushort"),
                            new BaseIntegerType(16, true , "short"),
                            new BaseIntegerType(8, false,  "byte"),
                            new BaseIntegerType(8, true,   "sbyte")
                        };
                        BaseIntegerType.PrimitiveType = BaseIntegerType.Types.Find(p => p.Name == "byte");
                        break;
                    default:
                        if (BaseIntegerType.Types == null || 
                            BaseIntegerType.CommonType == null ||
                            BaseIntegerType.CommonSignedType == null)
                            throw new InvalidOperationException("Если указанная битность не 64, 32, 16 или 8 вы должны сами указать все базовые типы");
                        break;
                }

                switch (value)
                {
                    case (64):
                        BaseIntegerType.CommonType = BaseIntegerType.Types.Find(p => p.Name == "ulong");
                        BaseIntegerType.CommonSignedType = BaseIntegerType.Types.Find(p => p.Name == "long");
                        break;

                    case (32):
                        BaseIntegerType.CommonType = BaseIntegerType.Types.Find(p => p.Name == "uint");
                        BaseIntegerType.CommonSignedType = BaseIntegerType.Types.Find(p => p.Name == "int");
                        break;

                    case (16):
                        BaseIntegerType.CommonType = BaseIntegerType.Types.Find(p => p.Name == "ulong");
                        BaseIntegerType.CommonSignedType = BaseIntegerType.Types.Find(p => p.Name == "short");
                        break;

                    case (8):
                        BaseIntegerType.CommonType = BaseIntegerType.Types.Find(p => p.Name == "byte");
                        BaseIntegerType.CommonSignedType = BaseIntegerType.Types.Find(p => p.Name == "sbyte");
                        break;
                }
            }
        }
    }
}
