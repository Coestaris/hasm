namespace HASMLib.Parser.SyntaxTokens.Structure
{
    public class Modifier
    {
        public bool ValueRequired;
        public string Name;
        public bool IsRequired;

        public string Value;

        public Modifier(bool isRequired, bool valueRequired, string name)
        {
            IsRequired = isRequired; 
            ValueRequired = valueRequired;
            Name = name;
        }

        public Modifier(Modifier modifier, string value)
        {
            IsRequired = modifier.IsRequired;
            ValueRequired = modifier.ValueRequired;
            Name = modifier.Name;
            Value = value;
        }
    }
}
