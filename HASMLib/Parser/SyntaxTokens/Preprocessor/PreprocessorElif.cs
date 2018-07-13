using HASMLib.Core.BaseTypes;
using HASMLib.Parser.Parser;
using HASMLib.Parser.SyntaxTokens.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    internal class PreprocessorElif : PreprocessorDirective
    {
        public PreprocessorElif()
        {
            Name = "elif";
            CanAddNewLines = false;
        }

        internal override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Count == 0)
            {
                error = new ParseError(ParseErrorType.Preprocessor_ElseWithoutPreviousConditionals);
                return;
            }

            if (enableStack.Skip(1).Contains(false))
            {
                error = null;
                return;
            }

            PreprocessorIf.AllowDefinedFunction = true;
            PreprocessorIf.defines = defines;

            input = ClearInput(input);

            var expError = Expression.Parse(input, out Expression exp, (p) =>
            {
                return new ObjectReference((Integer)0, ReferenceType.Define);
            });

            if (expError != null)
            {
                error = expError;
                return;
            }

            bool result = exp.Calculate(null).AsBool();
            enableStack.Pop();
            enableStack.Push(result);

            error = null;
            PreprocessorIf.AllowDefinedFunction = false;
        }

        //Для include
        internal override List<SourceLine> Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
