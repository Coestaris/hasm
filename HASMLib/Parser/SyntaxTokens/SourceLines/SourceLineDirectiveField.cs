using HASMLib.Parser.SyntaxTokens.SourceLines;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    class SourceLineDirectiveField : SourceLineDirective
    {
        public static string FindName = "field";

        public override RuleTarget Target => RuleTarget.Field;

        public SourceLineDirectiveField(SourceLineDirective line) : base(line)
        {
            Name = line.Name;
            Parameters = line.Parameters;
            RequireCodeBlock = false;
        }
    }
}
