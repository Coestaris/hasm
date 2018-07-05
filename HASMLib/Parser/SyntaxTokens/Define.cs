using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens
{
    public  class Define
    {
        public static Regex GeneralDefineNameRegex = new Regex(@"\D\w*");

        public bool IsParametric;

        public string Name;
        public string Value;
        
        public Define(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public Define(string name)
        {
            Name = name;
            Value = "";
        }
    }

    public class ParametricDefine : Define
    {
        public static Regex ParametricDefineRegex = new Regex(@"\D\w*\((\D\w*(,?\s*\w){0,}){1,}\)");

        public ParametricDefine(string name, string value) : base(name.Split('(')[0], value)
        {
            var parts = name.Split('(');

            var part = parts[1].Trim(')');
            part = part.Replace(" ", "");
            part = part.Replace("\t", "");
            part = part.Replace("\r", "");

            ParameterNames = part.Split(',').ToList();

            IsParametric = true;
        }

        public List<string> ParameterNames;

        public string Expand(List<string> parameters, out ParseError error)
        {
            if(parameters.Count != ParameterNames.Count)
            {
                error = new ParseError(ParseErrorType.Preprocessor_WrongParameterCount);
                return null;
            }

            string expanded = Value;
            for(int i = 0; i < parameters.Count; i++)
            {
                expanded = expanded.Replace(ParameterNames[i], parameters[i]);
            }

            error = null;
            return expanded;
        }
    }
}
