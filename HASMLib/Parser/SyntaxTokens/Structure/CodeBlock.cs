using HASMLib.Parser.SyntaxTokens.SourceLines;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    public class CodeBlock
    {
        public const string BlockOpened = "{";
        public const string BlockClosed = "}";

        public bool IsMixed;
        public SourceLineDirective ParentDirective;

        public List<CodeBlock> ChildBlocks;
        public List<SourceLine> RawLines;

        public CodeBlock(SourceLineDirective parent)
        {
            ParentDirective = parent;
            ChildBlocks = new List<CodeBlock>();
            RawLines = new List<SourceLine>();
        }
    }
}
