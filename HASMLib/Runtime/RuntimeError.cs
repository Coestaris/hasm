using HASMLib.Parser.SyntaxTokens.SourceLines;
using System.Text;

namespace HASMLib.Runtime
{
    public class RuntimeError
    {
        public RuntimeOutputCode Code;
        public SourceLine ErrorLine;

        public RuntimeError(RuntimeOutputCode code)
        {
            Code = code;
        }

        public RuntimeError(RuntimeOutputCode code, SourceLine errorLine)
        {
            Code = code;
            ErrorLine = errorLine;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Error {0}", Code);
            if (ErrorLine != null)
            {
                if (ErrorLine.LineIndex != -1) sb.AppendFormat(" at line {0}", ErrorLine.LineIndex);
                if (ErrorLine.FileName != null) sb.AppendFormat(" at {0}", ErrorLine.FileName);
            }
            return sb.ToString();
        }
    }
}
