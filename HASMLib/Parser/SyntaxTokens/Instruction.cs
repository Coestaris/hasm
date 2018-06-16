using HASMLib.Core;
using HASMLib.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static HASMLib.Parser.HASMParser;

namespace HASMLib.Parser.SyntaxTokens
{
    public abstract class Instruction
    {
        public UInt24 Index { get; protected set; }
        public Regex Name { get; protected set; }
        public int ParameterCount { get; protected set; }
        public List<InstructionParameterType> ParameterTypes { get; protected set; }

        public abstract void Apply(MemZone memZone, List<NamedConstant> constants, List<InstructionParameter> parameters, RuntimeMachine runtimeMachine);
    }
}
