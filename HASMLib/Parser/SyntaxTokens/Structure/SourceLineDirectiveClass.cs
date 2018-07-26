using HASMLib.Parser.SyntaxTokens.SourceLines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    class SourceLineDirectiveClass : SourceLineDirective
    {
        public static string FindName = "class";

        public SourceLineDirectiveClass(SourceLine line) : base(line)
        {
            RequireCodeBlock = true;
        }
    }
}
