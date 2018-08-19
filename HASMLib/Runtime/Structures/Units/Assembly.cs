using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures.Units
{
    public class Assembly : BaseStructure
    {
        internal Function _entryPoint;

        public List<Class> Classes;

        public Function GetEntryPointFunction()
        {
            return null;
        }

        public Assembly(BaseStructure Base) : base(Base.Name, Base.Modifiers, Base.AccessModifier, Base.Childs)
        {
            UsedTypes = new List<TypeReference>();
            Classes = new List<Class>();
            ParentAssembly = null;
            Target = RuleTarget.Assembly;
            Directive = Base.Directive;

            SetParentAssembly(Base.Childs);

            foreach (var item in Base.Childs)
            {
                Classes.Add(item as Class);
            }
        }

        private void SetParentAssembly(List<BaseStructure> childs)
        {
            foreach (var child in childs)
            {
                if (child.Target == RuleTarget.Class)
                    (child as Class)._fullName = null;

                child.ParentAssembly = this;
                if(child.Childs != null)
                    SetParentAssembly(child.Childs);
            }
        }

        public override string FullName
        {
            get 
			{
				return Name;
			}
        }

        public override string Signature
        {
            get 
			{
				return $"Assembly: {FullName}";
			}
				
        }

        public List<Field> AllFields { get; internal set; }
        public List<Class> AllClasses { get; internal set; }
        public List<Function> AllFunctions { get; internal set; }

        internal List<TypeReference> UsedTypes;
        private int _usedTypesCounter;

        internal int RegisterType(TypeReference reference)
        {
            string searchName = reference.Name;

            if (reference.Type == TypeReferenceType.Class && !reference.Name.StartsWith(Name))
                searchName = ToAbsoluteName(searchName);

            TypeReference type = UsedTypes.Find(p => p.Name == searchName);
            if(type != null)
            {
                reference.UniqueID = type.UniqueID;
                reference.Registered = true;
                reference.Name = searchName;
                return type.UniqueID;
            }

            UsedTypes.Add(reference);
            reference.Name = searchName;
            reference.Registered = true;
            reference.UniqueID = _usedTypesCounter++;

            return reference.UniqueID;
        }
    }
}
