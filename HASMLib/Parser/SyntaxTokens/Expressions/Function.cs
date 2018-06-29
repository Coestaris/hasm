using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    public class Function
    {
        public Func<long, long> UnaryFunc;
        public string FunctionString;

        public override string ToString()
        {
            return FunctionString;
        }

        public Function(string functionString, Func<long, long> unaryFunction)
        {
            UnaryFunc = unaryFunction;
            FunctionString = functionString;
        }
    }
}
