using HASMLib.Parser.SourceParsing;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorInclude : PreprocessorDirective
    {
        private static Regex Name1Regex = new Regex("^\\\".*\\\"");
        private static Regex Name2Regex = new Regex(@"^<.*>");

        public PreprocessorInclude()
        {
            Name = "include";
            CanAddNewLines = true;
        }

        internal override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            throw new NotImplementedException();
        }

        //Для include
        internal override List<SourceLine> Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return null;
            }

            input = ClearInput(input);

            if (!Name1Regex.IsMatch(input) && !Name2Regex.IsMatch(input))
            {
                error = new ParseError(ParseErrorType.Preprocessor_WrongNameFormat);
                return null;
            }

            input = input.Trim('\"', '<', '>');

            var result = recursiveFunc(input);
            if (result.error != null)
            {
                error = result.error;
                return null;
            }
            else
            {
                error = null;
                return result.sourceLines;
            }
        }
    }
}
