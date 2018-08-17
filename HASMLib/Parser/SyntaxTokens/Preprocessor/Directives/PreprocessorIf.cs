using HASMLib.Core.BaseTypes;
using HASMLib.Parser.Preprocessor;
using HASMLib.Parser.SyntaxTokens.Expressions;
using HASMLib.Parser.SyntaxTokens.Expressions.Exceptions;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor.Directives
{
    internal class PreprocessorIf : PreprocessorDirective
    {
        public static bool AllowDefinedFunction = false;
        public static List<Define> defines;


        public PreprocessorIf()
        {
            Name = "if";
            CanAddNewLines = false;
        }

        internal override void Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            if (enableStack.Contains(false))
            {
                error = null;
                return;
            }

            AllowDefinedFunction = true;
            PreprocessorIf.defines = defines;

            string strInput = input.AsSingleLine();
            strInput = ClearInput(strInput);

            var expError = Expression.Parse(strInput, out Expression exp, (token) =>
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
            enableStack.Push(result);

            error = null;
            AllowDefinedFunction = false;
        }

        //Для include
        internal override List<SourceLine> Apply(StringGroup input, Stack<bool> enableStack, List<Define> defines, out ParseError error, Func<string, PreprocessorParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
