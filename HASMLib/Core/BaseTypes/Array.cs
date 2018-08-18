using HASMLib.Runtime.Structures;
using System.Collections.Generic;
using System.Linq;

namespace HASMLib.Core.BaseTypes
{
    public class Array
    {
        public ArrayType Type;
        public List<Object> Collection;

        public string AsString()
        {
            BaseIntegerType charType;
            if (BaseIntegerType.CommonCharType.Base != 8)
                charType = BaseIntegerType.CommonCharType;
            else charType = new BaseIntegerType(8, false, "_tmp_char");

            List<Integer> chars = new List<Integer>();
            foreach (Object obj in Collection)
                if (obj.Type.Type == TypeReferenceType.Integer)
                    chars.AddRange(obj.IntegerValue.Cast(charType));
                else
                    chars.AddRange($"[{obj.Type.ToString()}]".Select(p => (Integer)p));

            return new string(chars.Select(p => (char)p).ToArray());
        }

        public Array(string str)
        {
            Type = StringType.DefaultStringType;
            Collection = new List<Object>();
            str.ToList().ForEach(p => 
            {
                Collection.Add(new Object(Type.BaseType)
                {
                    IntegerValue = BaseIntegerType.CommonCharType.Cast((Integer)p)[0]
                });
            });
        }

        public Array(ArrayType type)
        {
            Type = type;
        }
    }
}
