using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    internal struct UnknownLabelNameError
    {
        public string Name;
        public int ConstIndex;
        public ParseError ParseError;

        //Ссылки на обьекты во флеше!
        public NamedConstant namedConstant;
        public MemZoneFlashElementConstantDummy memZoneFlashElementConstant;

        public UnknownLabelNameError(string name, ParseError pe, int constIndex, NamedConstant namedConstant, MemZoneFlashElementConstantDummy memZoneFlashElementConstant)
        {
            ConstIndex = constIndex;
            Name = name;
            ParseError = pe;
            this.namedConstant = namedConstant;
            this.memZoneFlashElementConstant = memZoneFlashElementConstant;
        }
    }
}


