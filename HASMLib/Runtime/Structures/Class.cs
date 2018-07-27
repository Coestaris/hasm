using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures
{
    public class Class : BaseStructure
    {
        public const string NameSeparator = ".";

        public const string AbstractKeyword = "abstract";
        public const string SealedKeyword = "sealed";

        private string _fullName;

        private static void GetName(Class _class, string separator, ref string result)
        {
            result = _class.Name + (result == null ? "" : separator + result);
            if (_class.IsInner)
                GetName(_class.InnerParent, separator, ref result);
        }

        public override string FullName
        {
            get
            {
                if (_fullName == null)
                    GetName(this, NameSeparator, ref _fullName);
                return _fullName;
            }
        }

        public override string Signature
        {
            get => $"class({AccessModifier.ToString().ToLower()}) {FullName}";
        }

        public bool IsInner { get; private set; }
        public Class InnerParent { get; private set; }

        public Class(BaseStructure Base) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            Target = RuleTarget.Class;
            InnerClasses = new List<Class>();
            Functions = new List<Function>();
            Fields = new List<Field>();

            foreach (var child in Base.Childs)
            {
                if (child.Target == RuleTarget.Class)
                {
                    (child as Class).IsInner = true;
                    (child as Class).InnerParent = this;
                    InnerClasses.Add(child as Class);
                }

                if (child.Target == RuleTarget.Method)
                {
                    (child as Function).BaseClass = this;
                    Functions.Add(child as Function);
                }

                if (child.Target == RuleTarget.Field)
                {
                    (child as Field).BaseClass = this;
                    Fields.Add(child as Field);
                }
            }
        }

        public bool IsSealed => Modifiers.Exists(p => p.Name == AbstractKeyword);
        public bool IsAbstact => Modifiers.Exists(p => p.Name == SealedKeyword);

        public List<Class> InnerClasses { get; private set; }
        public List<Function> Functions { get; private set; }
        public List<Field> Fields { get; private set; }

    }
}
