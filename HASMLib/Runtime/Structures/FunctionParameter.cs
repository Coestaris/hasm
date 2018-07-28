namespace HASMLib.Runtime.Structures
{
    public struct FunctionParameter
    {
        public TypeReference Type;
        public string Name;

        public override string ToString()
        {
            return $"{Type} {Name}";
        }

        public FunctionParameter(TypeReference type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
