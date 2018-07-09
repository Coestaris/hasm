using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens
{
    public abstract class SourceLine
    {
        internal static Regex CommentRegex = new Regex(@";[\d\W\s\w]{0,}$");

        private const string CommentReplaceChar = "";
        private const char CommentTrimChar = ':';
        private readonly char[] StringCleanUpChars = { ' ', '\t', '\r' };

        public string FileName { get; protected set; }
        public int LineIndex { get; protected set; }
        public bool Enabled { get; protected set; }
        public string Comment { get; protected set; }

        protected void CleanUpLine(ref string input)
        {
            input = input.Trim(StringCleanUpChars);
        }

        protected void FindAndDeleteComment(ref string input)
        {
            //Поиск вхождений коментария в строке
            Match comment = CommentRegex.Match(input);

            //Если в строке был найден коментарий, то запомнить его,
            //удалив со строки
            if (comment.Success)
            {
                input = CommentRegex.Replace(input, CommentReplaceChar);
                Comment = comment.Value.TrimStart(CommentTrimChar);
            }
        }
    }
}
