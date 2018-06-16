using System;
using System.Collections.Generic;
using HASMLib.Parser.SyntaxTokens.Constants;
using HASMLib.Core.MemoryZone;
using HASMLib.Core;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens
{
    public class Constant
    {
        public Int64 Value;
        public LengthQualifier Length;

        internal Constant()
        {

        }

        internal Constant(Int64 value, LengthQualifier lq)
        {
            Length = lq;
            Value = value;
        }

        private static List<ConstantFormat> _formats = new List<ConstantFormat>()
        {
            new ConstantDecFormat(),
            new ConstantHexFormat(),
            new ConstantBinFormat()
        };

        public override string ToString()
        {
            return string.Format("Constant[{0}{1}]", Value, 
                    (Length == LengthQualifier.Single ? 's' :
                     Length == LengthQualifier.Double ? 'd' :
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

        public List<UInt12> ToUInt12()
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    return new List<UInt12>() { (UInt12)Value };
                case LengthQualifier.Double:
                    return ((UInt24)Value).ToUInt12().ToList();
                case LengthQualifier.Quad:
                    return ((UInt48)Value).ToUInt12().ToList();
            }
            return null;
        }

        public MemZoneFlashElementConstant ToFlashElement(int index)
		{
			switch (Length) 
			{
				case LengthQualifier.Single:
					return new MemZoneFlashElementConstantUInt12 ((UInt12)Value, index);
				case LengthQualifier.Double:
					return new MemZoneFlashElementConstantUInt24 ((UInt24)Value, index);
				case LengthQualifier.Quad:	
					return new MemZoneFlashElementConstantUInt48 ((UInt48)Value, index);
			}
			return null;
		}
    }
}
