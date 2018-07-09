using HASMLib.Parser.SyntaxTokens;
using System.Collections.Generic;

namespace HASMLib.Parser.Parser
{
    internal struct PreprocessorParseResult
    {
        public List<SourceLine> sourceLines;
        public ParseError error;

        public PreprocessorParseResult(List<SourceLine> sourceLines, ParseError error)
        {
            this.sourceLines = sourceLines;
            this.error = error;
        }
    }
}
