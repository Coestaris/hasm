﻿using HASMLib.Runtime.Structures;
using HASMLib.Runtime.Structures.Units;

namespace HASMLib.Core.BaseTypes
{
    public class StringType : ArrayType
    {
        public static StringType DefaultStringType = new StringType(null);
        public const string StringTypeKeyword = "string";

        public StringType(Assembly assembly) : base(new TypeReference(BaseIntegerType.CommonCharType, assembly)) { }
    }
}
