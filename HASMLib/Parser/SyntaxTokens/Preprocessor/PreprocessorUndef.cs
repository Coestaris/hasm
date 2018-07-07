using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorUndef: PreprocessorDirective
    {
        public PreprocessorUndef()
        {
            Name = "undef";
            CanAddNewLines = false;
        }

        protected override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            input = ClearInput(input);

            if (!PreprocessorIfdef.ArgumentRegex.IsMatch(input))
            {
                error = new ParseError(ParseErrorType.Preprocessor_NameExpected);
                return;
            }

            if(!defines.Exists(p => p.Name == input))
            {
                error = new ParseError(ParseErrorType.Preprocessor_UnknownDefineName);
                return;
            }

            defines.RemoveAll(p => p.Name == input);
            error = null;
        }

        //Для include
        protected override List<SourceLine> Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
