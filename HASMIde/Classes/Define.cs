using System;

namespace HASM.Classes
{
    [Serializable]
    public class Define
    {
        public string Name;
        public string Value;

        public Define(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public Define()
        {

        }
    }
}
