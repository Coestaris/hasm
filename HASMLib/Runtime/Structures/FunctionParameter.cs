namespace HASMLib.Runtime.Structures
{
    public struct FunctionParameter
    {
        public TypeReference Type;
        public string Name;

        public FunctionParameter(TypeReference type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
