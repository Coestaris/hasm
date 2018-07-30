using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System.Collections.Generic;

namespace HASMLib.Runtime.Structures
{
    public struct FunctionRuntimeCache
    {
        internal List<FlashElementExpression> Expressions;
        internal List<ConstantMark> Constants;
        internal List<FlashElementInstruction> Instructions;
        internal List<FlashElementVariable> Variables;
    }
}
