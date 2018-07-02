using System;

namespace HASMLib.Parser.SyntaxTokens.Expressions.Exceptions
{
    internal class UnknownOperatorException : Exception
    {
        public string OperatorName { get; private set; }

        public UnknownOperatorException(string operatorName)
        {
            OperatorName = OperatorName;
        }
    }
}
