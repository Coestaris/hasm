﻿using HASMLib.Parser.SyntaxTokens.Structure;

namespace HASMLib.Runtime.Structures.Units
{
    public class Field : BaseStructure
    {
        public const string TypeKeyword = "type";

        public bool IsStatic { get; set; }
        public Class BaseClass;
        internal TypeReference Type;

        public Field(BaseStructure Base) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            Target = RuleTarget.Field;
            Directive = Base.Directive;

            Modifier type = GetModifier(TypeKeyword);
            Type = new TypeReference(type.Value, null);

            IsStatic = GetModifier(StaticKeyword) != null;
        }

        public override string FullName
        {
            get 
			{
				return BaseClass.ToAbsoluteName (Name);
			}
        }

        public override string Signature
        {
            get 
			{
				return $"{Type} {FullName}";
			}
        }
    }
}
