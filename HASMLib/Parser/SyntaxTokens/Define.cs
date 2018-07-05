using System.Text.RegularExpressions;

namespace HASMLib.Parser.SyntaxTokens
{
    public class Define
    {
        public static Regex GeneralDefineNameRegex = new Regex(@"^\D\w*");

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
}
