﻿using HASMLib.Core;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    internal class ConstantFormat
    {
        public virtual Regex Regex { get; }

        public bool CheckMaxValues(long value, LengthQualifier qualifier)
        {
            switch (qualifier)
            {
                case LengthQualifier.Single:
                    return value > (long)HASMBase.SingleMaxValue;
                case LengthQualifier.Double:
                    return value > (long)HASMBase.DoubleMaxValue;
                case LengthQualifier.Quad:
                    return value > (long)HASMBase.QuadMaxValue;
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
                        return new ParseError(ParseErrorType.Syntax_Constant_UnknownConstantLengthQualifier);
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
