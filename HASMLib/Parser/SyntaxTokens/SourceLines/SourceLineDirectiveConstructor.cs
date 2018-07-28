using HASMLib.Parser.SyntaxTokens.SourceLines;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    class SourceLineDirectiveConstructor : SourceLineDirective
    {
        public static string FindName = "constructor";

        public override RuleTarget Target => RuleTarget.Constructor;

        public SourceLineDirectiveConstructor(SourceLineDirective line) : base(line)
        {
            Name = line.Name;
            Parameters = line.Parameters;
            RequireCodeBlock = true;
        }
    }
}
