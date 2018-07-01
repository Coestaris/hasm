using HASMLib.Core;
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

        public bool CheckMaxValues(long value, LengthQualifier qualifier)
        {
            switch (qualifier)
            {
                case LengthQualifier.Single:
                    return value > SMaxValue;
                case LengthQualifier.Double:
                    return value > DMaxValue;
                case LengthQualifier.Quad:
                    return value > QMaxValue;
                default:
                    return false;
            }
        }

        public ParseError Parse(string str, out Constant constant)
        {
            if (str.Last() == 's' || str.Last() == 'q' || (str.Last() == 'd' && str[str.Length - 2] == '_'))
            {

                char c = str.Last();
                str = str.Remove(str.Length - 2, 2);

                switch (c)
                {
                    case 's':
                        return Parse(str, LengthQualifier.Single, out constant);
                    case 'd':
                        return Parse(str, LengthQualifier.Double, out constant);
                    case 'q':
                        return Parse(str, LengthQualifier.Quad, out constant);
                    default:
                        constant = null;
                        return new ParseError(ParseErrorType.Constant_UnknownConstantLengthQualifier);
                }
            }
            else
            {
                return Parse(str, LengthQualifier.Single, out constant);
            }
        }

        protected virtual ParseError Parse(string str, LengthQualifier Length, out Constant constant) { constant = null; return null; }
    }
}
