using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib.Core.BaseTypes
{
    public class Array
    {
        ArrayType Type;

        List<Object> Collection;

        public Array(ArrayType type)
        {
            Type = type;
        }
    }
}
