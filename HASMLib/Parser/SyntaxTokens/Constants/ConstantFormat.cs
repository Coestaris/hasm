using HASMLib.Core.BaseTypes;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    internal class ConstantFormat
    {
        public virtual Regex Regex { get; }

        public bool CheckMaxValues(ulong value, BaseIntegerType type)
        {
            return (value > type.MaxValue) || ((long)value < type.MinValue);
        }

        public virtual ParseError Parse(string str, out Constant constant)
        {
            if (str.Contains('_'))
            {
                var parts = str.Split('_');
                if (parts.Length != 2)
                {
                    constant = null;
                    return new ParseError(ParseErrorType.Syntax_Constant_WrongFormat);
                }

                var type = BaseIntegerType.Types.Find(p => p.Name == parts[0]);

                if (type == null)
                {
                    constant = null;
                    return new ParseError(ParseErrorType.Syntax_Constant_WrongType);
                }

                return Parse(parts[1], type, out constant);
            }
            else
            {
                return Parse(str, BaseIntegerType.CommonType, out constant);
            }
        }

        protected virtual ParseError Parse(string str, BaseIntegerType type, out Constant constant) { constant = null; return null; }
    }
}
