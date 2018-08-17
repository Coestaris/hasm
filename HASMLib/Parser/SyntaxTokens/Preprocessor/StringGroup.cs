using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    public class StringGroup
    {
        public bool IsEmpty => Strings == null;

        public static StringGroup Empty = new StringGroup();

        private StringGroup() { }

        public StringGroup(IEnumerable<string> lines)
        {
            Strings = lines.ToList();
        }

        public StringGroup(string singleLine)
        {
            Strings = new List<string>()
            {
                singleLine
            };
        }

        public void Add(string str)
        {
            Strings.Add(str);
        }

        public List<string> Strings { get; private set; }

        public bool IsSingleLine => Strings.Count == 1;
        public bool IsMultilineDefine => Strings.Count > 1 && PreprocessorDirective.IsPreprocessorLine(Strings.First());

        internal string AsSingleLine()
        {
            return Strings[0];
        }

        public override string ToString()
        {
            return string.Join("\n", Strings);
        }
    }
}
