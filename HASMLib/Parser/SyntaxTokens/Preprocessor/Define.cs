using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens.Preprocessor
{
    public class Define
    {
        public static Regex GeneralDefineNameRegex = new Regex(@"^\D\w*");
        public static string FindBaseRegex = @"(^{0}(?=\W))|((?<=\W){0}(?=\W))|((?<=\W){0}$)";

        public bool IsFileSpecific;
        public Regex FindRegex { get; private set; }
        public bool IsParametric { get; protected set; }
        public string Name { get; private set; }
        public StringGroup Value { get; internal set; }
        public bool IsEmpty => Value.IsEmpty;

        public bool IsMultiline { get; internal set; }

        private void InitRegex()
        {
            FindRegex = new Regex(string.Format(FindBaseRegex, Name));
        }

        public override string ToString()
        {
            return
                $"{Name}" +
                $"{(IsParametric ? "(" + string.Join(",", (this as ParametricDefine).ParameterNames) + ")" : "")}" +
                $"{(Value.AsSingleLine().Length > 10 ? "[" + Value.AsSingleLine().Substring(0, 10) + "...]" : (Value.AsSingleLine().Length != 0 ? "[" + Value + "]" : ""))}";
        }

        public static ParseError ResolveDefines(List<Define> defines, ref StringGroup group, int index, string fileName)
        {
            string plainText = group.ToString();
            foreach (Define define in defines)
            {
                var matches = define.FindRegex.Matches(plainText);
                var commentIndex = plainText.IndexOf(';');

                if (matches.Count != 0)
                {
                    for (int matchIndex = matches.Count - 1; matchIndex >= 0; matchIndex--)
                    {
                        Match match = matches[matchIndex];

                        if (commentIndex != -1 && match.Index > commentIndex)
                            continue;

                        if (define.IsEmpty)
                        {
                            return new ParseError(ParseErrorType.Preprocessor_ReferenceToEmptyDefine, index, match.Index, fileName);
                        }

                        if (define.IsParametric)
                        {
                            Match parametricMatch = ParametricDefine.ParametricUsageRegex.Match(plainText);
                            if (parametricMatch.Success)
                            {
                                var subStr = plainText.Substring(parametricMatch.Index, parametricMatch.Length);
                                plainText = plainText.Remove(parametricMatch.Index, parametricMatch.Length);

                                var value = (define as ParametricDefine).Expand(subStr, out ParseError parseError);
                                if (parseError != null) return new ParseError(parseError.Type, index, parametricMatch.Index, fileName);

                                plainText = plainText.Insert(parametricMatch.Index, value.ToString());
                            }
                            else
                            {
                                return new ParseError(ParseErrorType.Preprocessor_WrongParametricDefineFormat, index, fileName);
                            }
                        }
                        else
                        {
                            plainText = plainText.Remove(match.Index, match.Length);
                            plainText = plainText.Insert(match.Index, define.Value.ToString());
                        }
                    }
                }
            }
            group = new StringGroup(plainText.Split('\n'));
            return null;
        }

        public Define(string name, StringGroup value)
        {
            Name = name;
            Value = value;
            InitRegex();
        }

        public Define(string name)
        {
            Name = name;
            Value = StringGroup.Empty;
            InitRegex();
        }
    }
}
