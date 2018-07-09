using HASMLib.Core.BaseTypes;
using HASMLib.Parser.SyntaxTokens.Expressions;
using System;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementExpression : MemZoneFlashElement
    {
        public override int FixedSize => 0; //TODO:

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Expression;

        public Expression Expression { get; private set; }

        public FDouble Index { get; private set; }

        public MemZoneFlashElementExpression(Expression expression, FDouble index)
        {
            Index = index;
            Expression = expression;
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
