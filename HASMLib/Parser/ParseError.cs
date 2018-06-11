namespace HASMLib.Parser
{

    public class ParseError
    {
        public override string ToString()
        {
            return string.Format("{0} at index {1}:{2}", Type.ToString(), Line, Index);
        }

        public ParseError(ParseErrorType type, int index)
        {
            Type = type;
            Index = index;
        }

        public ParseError(ParseErrorType type, int line, int index)
        {
            Type = type;
            Index = index;
			Line = line;
        }


        public ParseErrorType Type { get; set; }
        public int Line { get; set; }
        public int Index { get; set; }
    }
}