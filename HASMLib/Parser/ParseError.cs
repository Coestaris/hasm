using System.Text;

namespace HASMLib.Parser
{
    public class ParseError
    {
        private const int NullValue = -1;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Error {0}", Type);
            if (Line != NullValue) sb.AppendFormat(" at line {0}", Line);
            if (Index != NullValue) sb.AppendFormat(" at index {0}", Index);
            if (FileName != null) sb.AppendFormat(" at {0}", FileName);
            return sb.ToString();
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