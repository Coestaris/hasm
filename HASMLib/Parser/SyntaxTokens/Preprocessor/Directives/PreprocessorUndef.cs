using HASMLib.Parser.Preprocessor;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor.Directives
{
    internal class PreprocessorUndef : PreprocessorDirective
    {
        public PreprocessorUndef()
        {
            Name = "undef";
            CanAddNewLines = false;
        }

        internal override void Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            string strInput = input.AsSingleLine();
            strInput = ClearInput(strInput);

            if (!PreprocessorIfdef.ArgumentRegex.IsMatch(strInput))
            {
                error = new ParseError(ParseErrorType.Preprocessor_NameExpected);
                return;
            }

            if (!defines.Exists(p => p.Name == strInput))
            {
                error = new ParseError(ParseErrorType.Preprocessor_UnknownDefineName);
                return;
            }

            defines.RemoveAll(p => p.Name == strInput);
            error = null;
        }

        //Для include
        internal override List<SourceLine> Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
