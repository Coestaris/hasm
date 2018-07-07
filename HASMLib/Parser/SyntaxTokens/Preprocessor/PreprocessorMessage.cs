using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorMessage : PreprocessorDirective
    {
        public PreprocessorMessage()
        {
            Name = "message";
            CanAddNewLines = false;
        }

        protected override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            error = null;
        }

        //Для include
        protected override List<SourceLine> Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
