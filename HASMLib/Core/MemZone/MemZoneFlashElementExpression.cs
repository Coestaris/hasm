using HASMLib.Parser;
using HASMLib.Parser.SyntaxTokens.Expressions;
using System;
using System.Collections.Generic;

namespace HASMLib.Core.MemoryZone
{
    public class MemZoneFlashElementExpression : MemZoneFlashElement
    {
        public override int FixedSize => 0; //TODO:

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Expression;

        public List<ObjectReference> Dependencies;

        public Expression Expression; 

        public UInt24 Index;

        public MemZoneFlashElementExpression(Expression expression, UInt24 index)
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
