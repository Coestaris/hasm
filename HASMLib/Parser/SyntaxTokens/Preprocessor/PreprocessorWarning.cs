﻿using HASMLib.Parser.Parser;
using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorWarning : PreprocessorDirective
    {
        public PreprocessorWarning()
        {
            Name = "warning";
            CanAddNewLines = false;
        }

        internal override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            error = null;
        }

        //Для include
        internal override List<SourceLine> Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
