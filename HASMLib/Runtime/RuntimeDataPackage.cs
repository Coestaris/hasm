using HASMLib.Core;
using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System.Collections.Generic;

namespace HASMLib.Runtime
{
    public class RuntimeDataPackage
    {
        public MemZone MemZone;
        public List<ConstantMark> Constants;
        public List<FlashElementExpression> Expressions;
        public RuntimeMachine RuntimeMachine;
    }
}
