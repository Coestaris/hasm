﻿using HASMLib.Parser.Preprocessor;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor.Directives
{
    internal class PreprocessorMessage : PreprocessorDirective
    {
        public PreprocessorMessage()
        {
            Name = "message";
            CanAddNewLines = false;
        }

        internal override void Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            error = null;
        }

        //Для include
        internal override List<SourceLine> Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
