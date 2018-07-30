using HASMLib.Core.BaseTypes;
using HASMLib.Core.MemoryZone;

namespace HASMLib.Parser
{
    internal struct ConstantErrorMark
    {
        public string Name;
        public Integer ConstIndex;
        public ParseError ParseError;

        //Ссылки на обьекты во флеше!
        public ConstantMark namedConstant;
        public FlashElementConstantDummy memZoneFlashElementConstant;

        public ConstantErrorMark(string name, ParseError pe, Integer constIndex, ConstantMark namedConstant, FlashElementConstantDummy memZoneFlashElementConstant)
        {
            ConstIndex = constIndex;
            Name = name;
            ParseError = pe;
            this.namedConstant = namedConstant;
            this.memZoneFlashElementConstant = memZoneFlashElementConstant;
        }
    }
}


