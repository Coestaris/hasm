using HASMLib.Core;
using System;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    internal class ConstantDecFormat : ConstantFormat
    {
        private static Regex _regex = new Regex(@"^\d{1,30}(_[sdq]){0,1}$");

        public override Regex Regex => _regex;

        protected override ParseError Parse(string str, LengthQualifier Length, out Constant constant)
        {
            constant = new Constant();

            try
            {
                constant.Value = Convert.ToInt64(str);
            } catch(OverflowException)
            {
                return new ParseError(ParseErrorType.Constant_TooLong);
            }

            constant.Length = Length;

            if (CheckMaxValues(constant.Value, constant.Length))
                return new ParseError(ParseErrorType.Constant_BaseOverflow);

            return null;
        }
    }
}
