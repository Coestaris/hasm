using System;
using System.Collections.Generic;
using HASMLib.SyntaxTokens.Constants;

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
    }
}
