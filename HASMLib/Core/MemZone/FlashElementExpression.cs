using HASMLib.Core.BaseTypes;
using HASMLib.Parser.SyntaxTokens.Expressions;
using System;

namespace HASMLib.Core.MemoryZone
{
    public class FlashElementExpression : FlashElement
    {
        public Expression Expression { get; private set; }
        public Integer Index { get; private set; }

        public override int FixedSize => 0; //TODO:
        public override FlashElementType Type => FlashElementType.Expression;

        public FlashElementExpression(Expression expression, Integer index)
        {
            Index = index;
            Expression = expression;
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Expression[{Expression.TokenTree.RawValue}]";
        }
    }
}
