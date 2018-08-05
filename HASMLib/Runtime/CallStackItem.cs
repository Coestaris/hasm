using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;

namespace HASMLib.Runtime
{
    public class CallStackItem
    {
        public List<Variable> Locals { get; private set; }
        public Function RunningFunction;
        public Integer ProgramCounter;

        public CallStackItem(Function runningFunction, Integer programCounter)
        {
            Locals = new List<Variable>();
            RunningFunction = runningFunction;
            ProgramCounter = programCounter;
        }
    }
}