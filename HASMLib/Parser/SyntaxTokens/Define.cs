using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens
{
    public class Define
    {
        public static Regex GeneralDefineNameRegex = new Regex(@"^\D\w*");
        public static string FindBaseRegex = @"(^{0}(?=\W))|((?<=\W){0}(?=\W))|((?<=\W){0}$)";

        public Regex FindRegex { get; private set; }
        public bool IsParametric { get; protected set; }
        public string Name { get; private set; }
        public string Value { get; internal set; }
        public bool IsEmpty => string.IsNullOrEmpty(Value);

        private void InitRegex()
        {
            FindRegex = new Regex(string.Format(FindBaseRegex, Name));
        }

        public override string ToString()
        {
            return
                $"{Name}" +
                $"{(IsParametric ? "(" + string.Join(",", (this as ParametricDefine).ParameterNames) + ")" : "")}" +
                $"{(Value.Length > 10 ? "[" + Value.Substring(0, 10) + "...]" : (Value.Length != 0 ? "[" + Value + "]" : ""))}";
        }

        public static ParseError ResolveDefines(List<Define> defines, ref string line, int index, string fileName)
        {
            foreach (Define define in defines)
            {
                var matches = define.FindRegex.Matches(line);
                var commentIndex = line.IndexOf(';');

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
                            Match parametricMatch = ParametricDefine.ParametricUsageRegex.Match(line);
                            if (parametricMatch.Success)
                            {
                                var subStr = line.Substring(parametricMatch.Index, parametricMatch.Length);
                                line = line.Remove(parametricMatch.Index, parametricMatch.Length);

                                var newStr = (define as ParametricDefine).Expand(subStr, out ParseError parseError);
                                if (parseError != null) return new ParseError(parseError.Type, index, parametricMatch.Index, fileName);

                                line = line.Insert(parametricMatch.Index, newStr);
                            }
                            else
                            {
                                return new ParseError(ParseErrorType.Preprocessor_WrongParametricDefineFormat, index, fileName);
                            }
                        }
                        else
                        {
                            line = line.Remove(match.Index, match.Length);
                            line = line.Insert(match.Index, define.Value);
                        }
                    }
                }
            }
            return null;
        }

        public Define(string name, string value)
        {
            Name = name;
            Value = value;
            InitRegex();
        }

        public Define(string name)
        {
            Name = name;
            Value = "";
            InitRegex();
        }
    }
}
