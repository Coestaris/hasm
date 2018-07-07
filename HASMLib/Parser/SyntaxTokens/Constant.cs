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

        public static long TrimValue(long value, LengthQualifier lengthQualifier)
        {
            switch (lengthQualifier)
            {
                case LengthQualifier.Single:
                    return (UIntSingle)value;
                case LengthQualifier.Double:
                    return (UIntDouble)value;
                case LengthQualifier.Quad:
                    return (UIntDouble)value;
            }

            return 0;
        }

        public bool AsBool()
        {
            return Value == 1;
        }

        internal Constant() { }

        internal Constant(bool value) : this(value ? 1 : 0, LengthQualifier.Single) { }

        public static LengthQualifier GetQualifier(LengthQualifier a, LengthQualifier b)
        {
            return (LengthQualifier)Math.Max((int)a, (int)b);
        }

        internal Constant(MemZoneVariable variable)
        {
            Length = variable.Length;
            Value = variable.GetNumericValue();
        }

        internal Constant(Int64 value, LengthQualifier lq)
        {
            Length = lq;
            Value = TrimValue(value, lq);
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
            return new ParseError(ParseErrorType.Syntax_Constant_WrongFormat, 0);
        }

        public List<UIntSingle> ToUInt12()
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    return new List<UIntSingle>() { (UIntSingle)Value };
                case LengthQualifier.Double:
                    return ((UIntDouble)Value).ToUInt12().ToList();
                case LengthQualifier.Quad:
                    return ((UIntQuad)Value).ToUInt12().ToList();
            }
            return null;
        }

        public MemZoneFlashElementConstant ToFlashElement(int index)
		{
			switch (Length) 
			{
				case LengthQualifier.Single:
					return new MemZoneFlashElementConstantUInt12 ((UIntSingle)Value, index);
				case LengthQualifier.Double:
					return new MemZoneFlashElementConstantUInt24 ((UIntDouble)Value, index);
				case LengthQualifier.Quad:	
					return new MemZoneFlashElementConstantUInt48 ((UIntQuad)Value, index);
			}
			return null;
		}
    }
}
