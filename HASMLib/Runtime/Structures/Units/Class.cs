using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures.Units
{
    public class Class : BaseStructure
    {
        public const string AbstractKeyword = "abstract";
        public const string SealedKeyword = "sealed";
        internal string _fullName;
        
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
                {
                    GetName(this, NameSeparator, ref _fullName);
                    if(ParentAssembly != null)
                        _fullName = ParentAssembly.Name + NameSeparator + _fullName;
                }
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
            Directive = Base.Directive;

            InnerClasses = new List<Class>();
            Functions = new List<Function>();
            Fields = new List<Field>();

            foreach (var child in Base.Childs)
            {
                switch (child.Target)
                {
                    case RuleTarget.Class:
                        (child as Class).IsInner = true;
                        (child as Class).InnerParent = this;
                        InnerClasses.Add(child as Class);
                        break;
                    case RuleTarget.Method:
                        (child as Function).BaseClass = this;
                        Functions.Add(child as Function);
                        break;
                    case RuleTarget.Field:
                        (child as Field).BaseClass = this;
                        Fields.Add(child as Field);
                        break;
                    case RuleTarget.Constructor:
                        (child as Function).BaseClass = this;
                        (child as Function).RetType = new TypeReference(this);
                        break;
                    default:
                        break;
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
