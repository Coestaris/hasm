using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorError : PreprocessorDirective
    {
        public PreprocessorError()
        {
            Name = "error";
            CanAddNewLines = false;
        }

        protected override void Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error)
        {
            error = new ParseError(ParseErrorType.Preprocessor_UserDefinedError);
        }

        //Для include
        protected override List<SourceLine> Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
