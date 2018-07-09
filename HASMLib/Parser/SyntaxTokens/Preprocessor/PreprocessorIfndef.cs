using HASMLib.Parser.Parser;
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

        internal override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
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

            enableStack.Push(!defines.Exists(p => p.Name == input));

            error = null;
        }

        //Для include
        internal override List<SourceLine> Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
