using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorIfndef : PreprocessorDirective
    {
        public PreprocessorIfndef()
        {
            Name = "ifndef";
            CanAddNewLines = false;
        }

        protected override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            input = ClearInput(input);

            if (!PreprocessorIfdef.ArgumentRegex.IsMatch(input))
            {
                error = new ParseError(ParseErrorType.Preprocessor_NameExpected);
                return;
            }

            enableStack.Push(!defines.Exists(p => p.Name == input));

            error = null;
        }

        //Для include
        protected override List<SourceLine> Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
