using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime.Structures;
using System.Collections.Generic;

namespace HASMLib.Parser.SyntaxTokens.Constants
{
    public class Constant
    {
        private const string TrueValue = "true";
        private const string FalseValue = "false";

        public Integer IntValue;
        public Array ArrayValue;

        public TypeReferenceType Type;

        public bool AsBool()
        {
            if (Type == TypeReferenceType.Integer)
                return IntValue == (Integer)1;
            else
                return ArrayValue.AsString().ToLower() == TrueValue;
        }

        internal Constant(BaseIntegerType type)
        {
            Type = TypeReferenceType.Integer;
            IntValue = new Integer(0, type);
        }

        internal Constant(Array value)
        {
            ArrayValue = value;
            Type = TypeReferenceType.Array;
        }

        internal Constant(bool value) : this(value ? (Integer)1 : (Integer)0) { }

        internal Constant(Variable variable)
        {
            if (variable.Value.Type.Type == TypeReferenceType.Array)
            {
                Type = TypeReferenceType.Array;
                ArrayValue = variable.Value.ArrayValue;
            }
            else if (variable.Value.Type.Type == TypeReferenceType.Integer)
            {
                Type = TypeReferenceType.Integer;
                IntValue = variable.Value.IntegerValue;
            }
            else throw new System.ArgumentException("Wrong type given");
        }

        internal Constant(ulong value)
        {
            Type = TypeReferenceType.Integer;
            IntValue = new Integer(value, BaseIntegerType.CommonType);
        }

        internal Constant(Integer value)
        {
            Type = TypeReferenceType.Integer;
            IntValue = value;
        }

        private static List<ConstantFormat> _formats = new List<ConstantFormat>()
        {
            new ConstantDecFormat(),
            new ConstantHexFormat(),
            new ConstantBinFormat(),
            new ConstantStringFormat()
        };

        public override string ToString()
        {
            if (Type == TypeReferenceType.Array)
                return $"ArrayConstant[{ArrayValue.AsString()}]";
            else
                return $"IntValue[{IntValue}]";
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

        internal List<Integer> ToPrimitiveInt()
        {
            return IntValue.Cast(BaseIntegerType.PrimitiveType);
        }

        public FlashElementConstant ToFlashElement(Integer index)
        {
            return new FlashElementConstant(this, index);
        }
    }
}
