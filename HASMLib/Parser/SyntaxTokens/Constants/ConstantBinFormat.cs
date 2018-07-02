using HASMLib.Core;
using System;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    internal class ConstantBinFormat : ConstantFormat
    {
        private static Regex _regex = new Regex(@"^0[bB][0-1]{1,}(_[sdq]){0,1}$");

        public override Regex Regex => _regex;

        protected override ParseError Parse(string str, LengthQualifier Length, out Constant constant)
        {
            constant = new Constant();
            str = str.Remove(0, 2);

            try
            {
                constant.Value = Convert.ToInt64(str, 2);
            }
            catch (OverflowException)
            {
                return new ParseError(ParseErrorType.Syntax_Constant_TooLong);
            }

            constant.Length = Length;

            if (CheckMaxValues(constant.Value, constant.Length))
                return new ParseError(ParseErrorType.Syntax_Constant_BaseOverflow);

            return null;
        }
    }
}
