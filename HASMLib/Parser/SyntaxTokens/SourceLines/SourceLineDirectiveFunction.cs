using HASMLib.Parser.SyntaxTokens.SourceLines;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    class SourceLineDirectiveFunction : SourceLineDirective
    {
        public static string FindName = "function";

        public override RuleTarget Target => RuleTarget.Method;

        public SourceLineDirectiveFunction(SourceLineDirective line) : base(line)
        {
            Name = line.Name;
            Parameters = line.Parameters;
            RequireCodeBlock = true;
        }
    }
}
