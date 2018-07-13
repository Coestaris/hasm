using HASMLib.Core.BaseTypes;
using System;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    internal class ConstantBinFormat : ConstantFormat
    {
        private static Regex _regex = new Regex(@"^(\w+_)?0b[01]+$");

        public override Regex Regex => _regex;

        protected override ParseError Parse(string str, BaseIntegerType type, out Constant constant)
        {
            constant = new Constant();
            str = str.Remove(0, 2);
            ulong value = 0;

            try
            {
                value = Convert.ToUInt64(str, 2);
            }
            catch (OverflowException)
            {
                return new ParseError(ParseErrorType.Syntax_Constant_TooLong);
            }

            if (CheckMaxValues(value, type))
                return new ParseError(ParseErrorType.Syntax_Constant_BaseOverflow);

            constant.Value = new Integer(value, type);

            return null;
        }
    }
}
