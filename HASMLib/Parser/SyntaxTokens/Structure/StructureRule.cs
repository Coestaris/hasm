using HASMLib.Runtime.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Parser.SyntaxTokens.Structure
{
    public class StructureRule
    {
        private List<AccessModifier> _availableAccsessModifiers;
        private List<Modifier> _modifiers;
        private List<RuleTarget> _targets;


        public Type AssignedType;
        public List<AccessModifier> AvailableAccsessModifiers
        {
            set
            {
                AvailableAccsessModifiersStrings = value.Select(p => p.ToString().ToLower()).ToList();
                _availableAccsessModifiers = value;
            }
            get => _availableAccsessModifiers;
        }
        public RuleTarget Target;

        public List<Modifier> Modifiers
        {
            set
            {
                if (value == null) _modifiers = new List<Modifier>();
                else _modifiers = value;
            }
            get => _modifiers;
        }

        public List<RuleTarget> AllowedChilds
        {
            set
            {
                if (value == null) _targets = new List<RuleTarget>();
                else _targets = value;
            }
            get => _targets;
        }

        public bool AllowedInnerInstructions;

        public List<string> AvailableAccsessModifiersStrings;
    }
}
