using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorElse : PreprocessorDirective
    {
        public PreprocessorElse()
        {
            Name = "else";
            CanAddNewLines = false;
        }

        protected override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if(enableStack.Count == 0)
            {
                error = new ParseError(ParseErrorType.Preprocessor_ElseWithoutPreviousConditionals);
                return;
            }

            var last = enableStack.Pop();
            enableStack.Push(!last);

            error = null;
        }

        //Для include
        protected override List<SourceLine> Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
