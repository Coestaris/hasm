using HASMLib.Parser.SyntaxTokens.Preprocessor;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;

namespace HASMLib.Storage
{
    public class FileCache
    {
        public string AbsoluteFileName;

        public List<Class> CompiledClasses;
        public List<Define> CompiledDefines;

        public FileCache()
        {
            CompiledClasses = new List<Class>();
            CompiledDefines = new List<Define>();
        }
    }
}