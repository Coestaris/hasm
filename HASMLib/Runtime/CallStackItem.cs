using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures.Units;

namespace HASMLib.Runtime
{
    internal class CallStackItem
    {
        public Function RunningFunction;
        public Integer ProgramCounter;

        public CallStackItem(Function runningFunction, Integer programCounter)
        {
            RunningFunction = runningFunction;
            ProgramCounter = programCounter;
        }
    }
}