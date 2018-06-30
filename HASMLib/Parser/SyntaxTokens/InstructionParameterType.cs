using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens
{
    [Flags]
    public enum InstructionParameterType
    {
        Register = 1,
        Constant = 2,
        Expression = 4,
    }
}
