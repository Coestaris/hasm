using HASMLib.Parser.SyntaxTokens.Structure;

namespace HASMLib.Parser.SyntaxTokens.SourceLines
{
    public class SourceLineCodeBlockLimiter : SourceLine
    {
        public SourceLineCodeBlockLimiter(SourceLine line) : base(line.Input, line.LineIndex, line.FileName) { }

        public bool IsOpening;

        public static bool IsCodeBlockLimiter(string input)
        {
            var Input = input.Trim(StringCleanUpChars);
            return Input.StartsWith(CodeBlock.BlockClosed) || Input.StartsWith(CodeBlock.BlockOpened);
        }

        public ParseError Parse()
        {
            string input = Input;

            FindAndDeleteComment(ref input);
            CleanUpLine(ref input);

            IsOpening = input.StartsWith(CodeBlock.BlockOpened);
            return null;
        }
    }
}
