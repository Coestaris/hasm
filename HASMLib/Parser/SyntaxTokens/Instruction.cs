using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HASMLib.Core;

namespace HASMLib.Parser.SyntaxTokens
{
    public abstract class Instruction
    {
        public UInt24 Index { get; protected set; }
        public Regex Name { get; protected set; }
        public int ParameterCount { get; protected set; }
        public List<InstructionParameterType> ParameterTypes { get; protected set; }

        public abstract void Apply(MemZone memZone, List<InstructionParameter> parameters);
    }
}
