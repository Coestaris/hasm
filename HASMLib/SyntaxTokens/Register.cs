using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.SyntaxTokens
{
    class Register
    {
        public static ParseError Parse(string value, out Register register)
        {
            register = new Register();
            return null;
        }
    }
}
