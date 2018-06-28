using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens
{
    public abstract class SourceLine
    {
        private const string CommentReplaceChar = "";
        private const char CommentTrimChar = ':';
        private Regex CommentRegex = new Regex(@";[\d\W\s\w]{0,}$");
        private readonly char[] StringCleanUpChars = { ' ', '\t', '\r' };

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

        public string FileName;
        public int LineIndex;
        public bool Enabled;

        public string Comment;

        public abstract ParseError Parse(string input);
    }
}
