using HASMLib.Runtime.Structures;
using HASMLib.Runtime.Structures.Units;
using System.Text.RegularExpressions;

namespace HASMLib.Core.BaseTypes
{
    public class ArrayType
    {
        public string Name => ToString();

        public override string ToString()
        {
            return $"Array[{BaseType.ToString()}]";
        }

        public TypeReference BaseType;
        public static Regex ArrayTypeRegex = new Regex(@"array\[\w+\]");

        public ArrayType(TypeReference type)
        {
            BaseType = type;
        }

        public ArrayType(string input, Assembly assembly)
        {
            var baseType = input.Split('[')[1].Trim(']');
            BaseType = new TypeReference(baseType, assembly);
        }
    }
}
