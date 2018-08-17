using HASMLib.Parser.Preprocessor;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor.Directives
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

        internal override void Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            throw new NotSupportedException();
        }

        //Для include
        internal override List<SourceLine> Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return null;
            }

            string strInput = input.AsSingleLine();
            strInput = ClearInput(strInput);

            if (!Name1Regex.IsMatch(strInput) && !Name2Regex.IsMatch(strInput))
            {
                error = new ParseError(ParseErrorType.Preprocessor_WrongNameFormat);
                return null;
            }

            strInput = strInput.Trim('\"', '<', '>');

            var result = recursiveFunc(strInput);
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
