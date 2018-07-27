using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Runtime.Structures
{
    public class TypeReference
    {
        public string Name;

        public bool IsBaseInteger;
        public BaseIntegerType IntegerType;

        public bool IsClass;
        public Class ClassType;

        public TypeReference(string fullName)
        {
            var bit = BaseIntegerType.Types.Find(p => p.Name == fullName);
            if(bit != null)
            {
                IsBaseInteger = true;
                IntegerType = bit;
                return;
            }

            IsClass = true;
        }
    }
}
