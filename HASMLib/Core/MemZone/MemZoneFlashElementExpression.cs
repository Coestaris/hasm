using HASMLib.Parser.SyntaxTokens.Expressions;
using System;
using System.Collections.Generic;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Core.MemoryZone
{
    class MemZoneFlashElementExpression : MemZoneFlashElement
    {
        public override int FixedSize => 0; //TODO:

        public override MemZoneFlashElementType Type => MemZoneFlashElementType.Expression;

        public List<ObjectReference> Dependencies;

        public Expression Expression; 

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
