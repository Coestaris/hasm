using HASMLib.Parser.SourceParsing;
using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorError : PreprocessorDirective
    {
        public PreprocessorError()
        {
            Name = "error";
            CanAddNewLines = false;
        }

        internal override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            error = new ParseError(ParseErrorType.Preprocessor_UserDefinedError);
        }

        //Для include
        internal override List<SourceLine> Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
