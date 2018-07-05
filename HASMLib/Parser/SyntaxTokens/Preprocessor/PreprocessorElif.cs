using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.SourceLines.Preprocessor
{
    internal class PreprocessorElif : PreprocessorDirective
    {
        public PreprocessorElif()
        {
            Name = "elif";
            CanAddNewLines = false;
        }

        protected override void Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error)
        {
            throw new NotImplementedException();
        }

        //Для include
        protected override List<SourceLine> Apply(string input, Stack<bool> enableList, List<Define> defines, out ParseError error, Func<string, RecursiveParseResult> recursiveFunc)
        {
            throw new NotSupportedException();
        }
    }
}
