using System;
using System.Collections.Generic;
using HASMLib.SyntaxTokens.Constants;
using HASMLib.Core.MemoryZone;
using HASMLib.Core;

namespace HASMLib.SyntaxTokens
{
    public class Constant
    {
        public Int64 Value;
        public ConstantLengthQualifier Length;

        internal Constant() { }

        private static List<ConstantFormat> _formats = new List<ConstantFormat>()
        {
            new ConstantDecFormat(),
            new ConstantHexFormat(),
            new ConstantBinFormat()
        };

        public override string ToString()
        {
            return string.Format("Constant[{0}{1}]", Value, 
                    (Length == ConstantLengthQualifier.Single ? 's' :
                     Length == ConstantLengthQualifier.Double ? 'd' :
                     'q'));
        }

        public static ParseError Parse(string value, out Constant constant)
        {
            foreach (var item in _formats)
            {
                if (item.Regex.IsMatch(value))
                    return item.Parse(value, out constant);
            }

            constant = null;
            return new ParseError(ParseErrorType.Constant_WrongFormat, 0);
        }

		public MemZoneFlashElementConstant ToFlashElement(int index)
		{
			switch (Length) 
			{
				case ConstantLengthQualifier.Single:
					return new MemZoneFlashElementConstantUInt12 ((UInt12)Value, index);
				case ConstantLengthQualifier.Double:
					return new MemZoneFlashElementConstantUInt24 ((UInt24)Value, index);
				case ConstantLengthQualifier.Quad:	
					return new MemZoneFlashElementConstantUInt48 ((UInt48)Value, index);
			}
			return null;
		}
    }
}
