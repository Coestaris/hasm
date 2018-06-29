using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Parser.SyntaxTokens.Expressions
{
    public class Operand
    {
        public long Value;

        public bool AsBool => Value == 1 ? true : false;

        public Operand(long value)
        {
            Value = value;
        }
    }
}
