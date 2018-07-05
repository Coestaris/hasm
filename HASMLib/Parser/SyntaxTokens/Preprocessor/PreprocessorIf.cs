using HASMLib.Parser.SyntaxTokens.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
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

        protected override void Apply(string input, Stack<bool> enableStack, List<Define> defines, out ParseError error)
        {
            AllowDefinedFunction = true;
            PreprocessorIf.defines = defines;

            input = ClearInput(input);

            var expError = Expression.Parse(input, out Expression exp, (p) =>
            {
                return new ObjectReference(0, ReferenceType.Define);
            });

            if(expError != null)
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
        protected override List<SourceLine> Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
