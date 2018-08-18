using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures.Units
{
    public class Class : BaseStructure
    {
        public const string AbstractKeyword = "abstract";
        public const string SealedKeyword = "sealed";
        public const string ExtendsKeyword = "extends";
        internal string _fullName;
        
        private static void GetName(Class _class, string separator, ref string result)
        {
            result = _class.Name + (result == null ? "" : separator + result);
            if (_class.IsInner)
                GetName(_class.InnerParent, separator, ref result);
        }

        public static bool operator ==(Class a, Class b)
        {
            if (a is null && b is null)
                return true;

            if ((a is null && !(b is null)) || (!(a is null) && b is null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Class a, Class b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Class _class && Signature == _class.Signature;
        }

        public override int GetHashCode()
        {
            var hashCode = 1409748649;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Signature);
            return hashCode;
        }

        public override string FullName
        {
            get
            {
                if (_fullName == null)
                {
                    GetName(this, NameSeparator, ref _fullName);
                    if(ParentAssembly != null)
                        _fullName = ParentAssembly.ToAbsoluteName(_fullName);
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
        public bool IsSealed => Modifiers.Exists(p => p.Name == AbstractKeyword);
        public bool IsAbstact => Modifiers.Exists(p => p.Name == SealedKeyword);

        public List<Class> InnerClasses { get; private set; }
        public List<Function> Functions { get; private set; }
        public List<Field> Fields { get; private set; }
        public List<Function> Constructors { get; private set; }

        public Dictionary<int, Object> StaticFields;
        public List<Class> Extends;

        public Class(BaseStructure Base) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            Extends = new List<Class>();
            StaticFields = new Dictionary<int, Object>();
            Target = RuleTarget.Class;
            Directive = Base.Directive;

            Constructors = new List<Function>();    
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
                        {
                            Field field = child as Field;
                            field.BaseClass = this;
                            Fields.Add(child as Field);
                        }
                        break;
                    case RuleTarget.Constructor:
                        (child as Function).BaseClass = this;
                        (child as Function).RetType = new TypeReference(this, null);
                        Constructors.Add(child as Function);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
