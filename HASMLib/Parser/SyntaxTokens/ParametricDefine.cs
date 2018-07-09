using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens
{
    public class ParametricDefine : Define
    {
        public static Regex ParametricDefineRegex = new Regex(@"^[A-Za-z_](?:(?!\W)\w)*\(([A-Za-z_](?:(?!\W\s)\w)*(, *?[A-Za-z_](\w)*)*)+\)");
        public static Regex ParametricUsageRegex = new Regex(@"[A-Za-z_](?:(?!\W)\w)*\(([^(),]+(, *?[^(),]+)*)+\)");

        public List<string> ParameterNames { get; private set; }

        private List<Regex> ParameterRegexes;

        public ParametricDefine(string name, string value) : base(name.Split('(')[0], value)
        {
            var parts = name.Split('(');

            var part = parts[1].Trim(')');
            part = part.Replace(" ", "");
            part = part.Replace("\t", "");
            part = part.Replace("\r", "");

            ParameterNames = part.Split(',').ToList();
            ParameterRegexes = new List<Regex>();

            foreach (var item in ParameterNames)
                ParameterRegexes.Add(new Regex(string.Format(FindBaseRegex, item)));

            IsParametric = true;
        }
        public string Expand(string input, out ParseError error)
        {
            var parts = input.Split('(');
            var part = parts[1].Trim(')');
            part = part.Replace(" ", "");
            part = part.Replace("\t", "");
            part = part.Replace("\r", "");

            var parameters = part.Split(',').ToList();

            if (parameters.Count != ParameterNames.Count)
            {
                error = new ParseError(ParseErrorType.Preprocessor_WrongParameterCount);
                return null;
            }

            string expanded = Value;
            for(int i = 0; i < parameters.Count; i++)
            {
                var matches = ParameterRegexes[i].Matches(expanded);
                foreach (Match match in matches)
                {
                    expanded = expanded.Remove(match.Index, match.Length);
                    expanded = expanded.Insert(match.Index, parameters[i]);
                }
            }

            error = null;
            return expanded;
        }
    }
}
