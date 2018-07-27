using HASMLib.Parser.SyntaxTokens.SourceLines;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    class SourceLineDirectiveAssembly : SourceLineDirective
    {
        public static string FindName = "assembly";

        public override RuleTarget Target => RuleTarget.Assembly;

        public SourceLineDirectiveAssembly(SourceLineDirective line) : base(line)
        {
            Name = line.Name;
            Parameters = line.Parameters;
            RequireCodeBlock = true;
        }
    }
}
