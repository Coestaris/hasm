using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser.SyntaxTokens.Constants;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens
{
    public class Constant
    {
        public Integer Value;

        public bool AsBool()
        {
            return Value == (Integer)1;
        }

        internal Constant() { }

        internal Constant(bool value) : this(value ? (Integer)1 : (Integer)0) { }

        internal Constant(MemZoneVariable variable)
        {
            Value = variable.Value;
        }

        internal Constant(ulong value)
        {
            Value = new Integer(value, BaseIntegerType.CommonType);
        }

        internal Constant(Integer value)
        {
            Value = value;
        }

        private static List<ConstantFormat> _formats = new List<ConstantFormat>()
        {
            new ConstantDecFormat(),
            new ConstantHexFormat(),
            new ConstantBinFormat()
        };

        public override string ToString()
        {
            return string.Format("Constant[{0}]", Value.ToString());
        }

        public static ParseError Parse(string value, out Constant constant)
        {
            foreach (var item in _formats)
            {
                if (item.Regex.IsMatch(value))
                    return item.Parse(value, out constant);
            }

            constant = null;
            return new ParseError(ParseErrorType.Syntax_Constant_WrongFormat, 0);
        }

        internal List<Integer> ToPrimitive()
        {
            return Value.Cast(BaseIntegerType.PrimitiveType);
        }

        public MemZoneFlashElementConstant ToFlashElement(Integer index)
        {
            return new MemZoneFlashElementConstant(Value, index);
        }
    }
}
