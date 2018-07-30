using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.SourceLines
{
    public class SourceLine
    {
        internal static Regex CommentRegex = new Regex(@";[\d\W\s\w]{0,}$");

        private const string CommentReplaceChar = "";
        private const char CommentTrimChar = ':';
        protected static readonly char[] StringCleanUpChars = { ' ', '\t', '\r' };

        public string FileName { get; protected set; }
        public int LineIndex { get; protected set; }
        public bool Enabled { get; protected set; }
        public string Comment { get; protected set; }

        public string Input;
        public virtual bool IsEmpty
        {
            get
            {
                FindAndDeleteComment(ref Input);
                return string.IsNullOrEmpty(Comment) ? 
                    string.IsNullOrWhiteSpace(Input) : 
                    string.IsNullOrWhiteSpace(Input.Replace(Comment, ""));
            }
        }

        public SourceLine(string input, int index = -1, string filename = null)
        {
            Input = input;
            LineIndex = index;
            FileName = filename;
        }

        public override string ToString()
        {
            return $"Source line: {Input}";
        }

        protected void CleanUpLine(ref string input)
        {
            input = input.Trim(StringCleanUpChars);
        }

        protected void FindAndDeleteComment(ref string input)
        {
            if (Comment != null)
                return;

            //Поиск вхождений коментария в строке
            Match comment = CommentRegex.Match(input);

            //Если в строке был найден коментарий, то запомнить его,
            //удалив со строки
            if (comment.Success)
            {
                input = CommentRegex.Replace(input, CommentReplaceChar);
                Comment = comment.Value.TrimStart(CommentTrimChar);
            } else
            {
                Comment = "";
            }
        }
    }
}
