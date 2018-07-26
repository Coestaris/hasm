using HASMLib.Core.BaseTypes;
using HASMLib.Parser.SourceParsing;
using HASMLib.Parser.SyntaxTokens.Expressions;
using HASMLib.Parser.SyntaxTokens.Expressions.Exceptions;
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


            var expError = Expression.Parse(input, out Expression exp, (token) =>
            {
                if (token.UnaryFunction == null || token.UnaryFunction.FunctionString != "defined")
                {
                    var def = defines.Find(p => p.Name == token.RawValue);
                    if (def == null) throw new WrongTokenException();
                    token.RawValue = def.Value;

                    if (!token.IsSimple)
                    {
                        Expression.Parse(token.RawValue, out Expression expression);
                        token.Subtokens = expression.TokenTree.Subtokens;
                    }

                    return null;
                }

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
