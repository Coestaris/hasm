using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.SyntaxTokens
{
    public class InstructionParameter
    {
        public InstructionParameterType Type { get; private set; }

        private Constant _constant;
        private Register _register;

        public object GetValue()
        {
            if (Type == InstructionParameterType.Register)
                return _register;
            else return _constant;
        }

        private InstructionParameter() { }

        public static InstructionParameter Parse(InstructionParameterType type, string value)
        {
            InstructionParameter result = new InstructionParameter();
            if (type == InstructionParameterType.Constant)
            {
                if (Constant.Parse(value, out result._constant) == null)
                    return null;
            } else
            {
                if (Register.Parse(value, out result._register) == null)
                    return null;
            }
            return result;
        }
    }
}
