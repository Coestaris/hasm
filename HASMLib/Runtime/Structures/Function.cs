using HASMLib.Core.MemoryZone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Runtime.Structures
{
    public class Function
    {
        public Class BaseClass;

        public AccessModifier AccessModifier;

        public string Name;

        public List<MemZoneFlashElement> ByteCode;
    }
}
