using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Runtime.Structures
{
    public class Class : BaseStructure
    {
        public string Name;

        public AccessModifier AccessModifier;

        public List<Class> Parents;

        public List<Function> Functions;
    }
}
