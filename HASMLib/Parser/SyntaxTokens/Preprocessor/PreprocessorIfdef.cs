using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorIfdef : PreprocessorDirective
    {
        internal static Regex ArgumentRegex = new Regex(@"^[A-Za-z_]\w*$"); 

        public PreprocessorIfdef()
        {
            Name = "ifdef";
            CanAddNewLines = false;
        }

        protected override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            input = ClearInput(input);

            if(!ArgumentRegex.IsMatch(input))
            {
                error = new ParseError(ParseErrorType.Preprocessor_NameExpected);
                return;
            }

            enableStack.Push(defines.Exists(p => p.Name == input));

            error = null;
        }

        //Для include
        protected override List<SourceLine> Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
