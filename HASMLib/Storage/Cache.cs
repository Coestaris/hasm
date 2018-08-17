using HASMLib.Parser;
using HASMLib.Parser.SourceParsing;
using HASMLib.Parser.SyntaxTokens.Preprocessor;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;
using System.IO;

namespace HASMLib.Storage
{
    public class Cache
    {
        public HASMMachine parrent;

        public void AddFileNames(string dirName, string pattern = "*.*")
        {
            PathsToCache.AddRange(Directory.GetFiles(dirName, pattern));
        }

        public void ClearCache()
        {
            FileCache = new Dictionary<string, FileCache>();
        }

        public ParseError BuildCache()
        {
            foreach (var file in PathsToCache)
            {
                HASMSource source = new HASMSource(parrent, new FileInfo(file));
                ParseTaskRunner runner = new ParseTaskRunner(source);
                runner.Run();

                if (runner.Status != ParseTaskStatus.Ok)
                    return runner.Tasks[runner.FailedTaskIndex].Error;

                FileCache.Add(file, new FileCache()
                {
                    AbsoluteFileName = file,
                    CompiledClasses = source.Assembly == null ? new List<Class>() : source.Assembly.Classes,
                    CompiledDefines = PreprocessorDirective.defines ?? new List<Define>()
                });
            }

            return null;
        }

        public Cache(HASMMachine machine)
        {
            PathsToCache = new List<string>();

            parrent = machine;
            ClearCache();
        }

        public List<string> PathsToCache;
        public Dictionary<string, FileCache> FileCache;
    }
}
