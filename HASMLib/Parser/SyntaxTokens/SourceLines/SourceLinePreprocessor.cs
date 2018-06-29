using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.SourceLines
{
    /*
        #define name[(arg, ...)] [value]
        #undef name
        #ifdef name
        #ifndef name
        #if <conditional>
        #elif <conditional>
        
        #else 
        #endif
        
        #error
        #warning 
        #message
        
        #include

        # (empty directive)
    */

    class SourceLinePreprocessor : SourceLine
    {
        public override ParseError Parse(string input)
        {
            throw new NotImplementedException();
        }
    }
}
