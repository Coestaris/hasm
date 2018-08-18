using HASMLib.Core.BaseTypes;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    class ConstantStringFormat : ConstantFormat
    {
        public override Regex Regex => new Regex("\\\".*\\\"");

        public override ParseError Parse(string str, out Constant constant)
        {
            str = str.Trim('"');
            constant = new Constant(new Array(str));
        }
    }
}
