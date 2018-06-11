using System;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    internal class ConstantHexFormat : ConstantFormat
    {
        private static Regex _regex = new Regex(@"^0[xX][0-9A-Fa-f]{1,15}(_[sdq]){0,1}$");

        public override Regex Regex => _regex;

        protected override ParseError Parse(string str, ConstantLengthQualifier Length, out Constant constant)
        {
            constant = new Constant();
            str = str.Remove(0, 2);

            try
            {
                constant.Value = Convert.ToInt64(str, 16);
            }
            catch (OverflowException)
            {
                return new ParseError(ParseErrorType.Constant_TooLong, 0);
            }

            constant.Length = Length;

            if (CheckMaxValues(constant.Value, constant.Length))
                return new ParseError(ParseErrorType.Constant_BaseOverflow, 0);

            return null;
        }
    }
}
