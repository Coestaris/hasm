using HASMLib.Core;
using HASMLib.Core.MemoryZone;
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

        protected MemZoneVariable GetVar(MemZone mz, UInt24 index)
        {
            return mz.RAM.Find(p => p.Index == index);
        }

        protected NamedConstant GetConst(List<NamedConstant> constants, UInt24 index)
        {
            return constants.Find(p => p.Index == index);
        }

        public void RuntimeMachineJump(UInt24 position, RuntimeMachine runtimeMachine)
        {
            var localIndex = position;
            var globalIndex = runtimeMachine.GetGlobalInstructionIndexByLocalOne(localIndex);
            runtimeMachine.CurrentPosition = (UInt24)(globalIndex - 1);
        }

        public abstract RuntimeOutputCode Apply(MemZone memZone, List<NamedConstant> constants, List<ObjectReference> parameters, RuntimeMachine runtimeMachine);
    }
}
