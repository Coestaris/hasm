using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    internal class ConstantFormat
    {
        public virtual Regex Regex { get; }

        protected const long SMaxValue = 0xFFF;
        protected const long DMaxValue = 0xFFFFF;
        protected const long QMaxValue = 0xFFFFFFFFFFFF;

        public bool CheckMaxValues(long value, ConstantLengthQualifier qualifier)
        {
            switch (qualifier)
            {
                case ConstantLengthQualifier.Single:
                    return value > SMaxValue;
                case ConstantLengthQualifier.Double:
                    return value > DMaxValue;
                case ConstantLengthQualifier.Quad:
                    return value > QMaxValue;
                default:
                    return false;
            }
        }

        public ParseError Parse(string str, out Constant constant)
        {
            if (char.IsDigit(str.Last()))
            {
                return Parse(str, ConstantLengthQualifier.Single, out constant);
            } else
            {
                char c = str.Last();
                str = str.Remove(str.Length - 2, 2);

                switch (c)
                {
                    case 's':
                        return Parse(str, ConstantLengthQualifier.Single, out constant);
                    case 'd':
                        return Parse(str, ConstantLengthQualifier.Double, out constant);
                    case 'q':
                        return Parse(str, ConstantLengthQualifier.Quad, out constant);
                    default:
                        constant = null;
                        return new ParseError(ParseErrorType.Constant_UnknownConstantLengthQualifier);
                }
            }
        }

        protected virtual ParseError Parse(string str, ConstantLengthQualifier Length, out Constant constant) { constant = null; return null; }
    }
}
