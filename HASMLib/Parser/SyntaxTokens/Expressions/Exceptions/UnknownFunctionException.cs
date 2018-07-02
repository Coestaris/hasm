using System;

namespace HASMLib.Parser.SyntaxTokens.Expressions.Exceptions
{
    internal class UnknownFunctionException : Exception
    {
        public string FuncName {get; private set;}

        public UnknownFunctionException(string funcName)
        {
            FuncName = funcName;
        }
    }
}
