namespace HASMLib.Parser
{

    public class ParseError
    {
		private const int NullValue = -1;

        public override string ToString()
        {
			if 		(Line != NullValue && Index != NullValue) 
				return string.Format("Error {0} at line: {1}, index: {2}", Type.ToString(), Line, Index);
			else if (Line == NullValue && Index != NullValue) 
				return string.Format("Error {0} at index {1}", Type.ToString(), Index);
			else if (Line != NullValue && Index == NullValue) 
				return string.Format("Error {0} at line {1}", Type.ToString(), Line);
			else 
				return string.Format("Error {0}", Type.ToString());
        }

		public ParseError(ParseErrorType type)
		{
			Type = type;
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

        public ParseError(ParseErrorType type, int line, int index, string fileName)
        {
            Type = type;
            Index = index;
            Line = line;
            FileName = fileName;
        }

        public ParseError(ParseErrorType type, int line, string fileName)
        {
            Type = type;
            Line = line;
            FileName = fileName;
        }

        public string FileName { get; set; }
        public ParseErrorType Type { get; set; }
		public int Line { get; set; } = NullValue;
		public int Index { get; set; } = NullValue;
    }
}