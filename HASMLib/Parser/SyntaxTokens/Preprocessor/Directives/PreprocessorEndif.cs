﻿using HASMLib.Parser.Preprocessor;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor.Directives
{
    internal class PreprocessorEndif : PreprocessorDirective
    {
        public PreprocessorEndif()
        {
            Name = "endif";
            CanAddNewLines = false;
        }

        internal override void Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Count == 0)
            {
                error = new ParseError(ParseErrorType.Preprocessor_EndifWithoutPreviousConditionals);
                return;
            }

            if (enableStack.Skip(1).Contains(false))
            {
                error = null;
                return;
            }

            enableStack.Pop();
            error = null;
        }

        //Для include
        internal override List<SourceLine> Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
