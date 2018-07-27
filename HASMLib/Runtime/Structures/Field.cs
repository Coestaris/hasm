using HASMLib.Parser.SyntaxTokens.Structure;

namespace HASMLib.Runtime.Structures
{
    public class Field : BaseStructure
    {
        public const string TypeKeyword = "type";

        public Class BaseClass;
        public string Type;

        public Field(BaseStructure Base) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            Target = RuleTarget.Field;
            Modifier type = GetModifier(TypeKeyword);
            Type = type.Value;
        }

        public override string FullName
        {
            get => BaseClass.FullName + Class.NameSeparator + Name;
        }

        public override string Signature
        {
            get => $"{Type} {FullName}";
        }
    }
}
