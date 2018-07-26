using HASMLib.Parser.SyntaxTokens.SourceLines;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    class SourceLineDirectiveClass : SourceLineDirective
    {
        public static string FindName = "class";

        public override RuleTarget Target => RuleTarget.Class;

        public SourceLineDirectiveClass(SourceLineDirective line) : base(line)
        {
            Name = line.Name;
            Parameters = line.Parameters;
            RequireCodeBlock = true;
        }
    }
}
