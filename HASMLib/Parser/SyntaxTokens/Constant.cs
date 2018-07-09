using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens
{
    public class Constant
    {
        public Int64 Value { get; internal set; }
        public LengthQualifier Length { get; internal set; }

        public static long TrimValue(long value, LengthQualifier lengthQualifier)
        {
            switch (lengthQualifier)
            {
                case LengthQualifier.Single:
                    return (FSingle)value;
                case LengthQualifier.Double:
                    return (FDouble)value;
                case LengthQualifier.Quad:
                    return (FDouble)value;
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

        public List<FSingle> ToSingle()
        {
            switch (Length)
            {
                case LengthQualifier.Single:
                    return new List<FSingle>() { (FSingle)Value };
                case LengthQualifier.Double:
                    return ((FDouble)Value).ToSingle().ToList();
                case LengthQualifier.Quad:
                    return ((FQuad)Value).ToSingle().ToList();
            }
            return null;
        }

        public MemZoneFlashElementConstant ToFlashElement(int index)
		{
			switch (Length) 
			{
				case LengthQualifier.Single:
					return new MemZoneFlashElementConstantSingle ((FSingle)Value, index);
				case LengthQualifier.Double:
					return new MemZoneFlashElementConstantDouble ((FDouble)Value, index);
				case LengthQualifier.Quad:	
					return new MemZoneFlashElementConstantQuad ((FQuad)Value, index);
			}
			return null;
		}
    }
}
