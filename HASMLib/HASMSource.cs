using HASMLib.Parser.SyntaxTokens.SourceLines;
using HASMLib.Parser.SyntaxTokens.Structure;
using HASMLib.Runtime.Structures.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HASMLib
{
    public class HASMSource
    {
        internal List<SourceLine> _lines;
        internal CodeBlock _parentBlock;

        public string Source { get; set; }
        public string BaseFilename { get; set; }
        public string WorkingDirectory { get; set; }
        public HASMMachine Machine { get; set; }

        public TimeSpan ParseTime { get; internal set; }

        public HASMSource(HASMMachine machine, FileInfo fileName, string workingDirectory = null)
        {
            WorkingDirectory = workingDirectory;
            BaseFilename = fileName.FullName;
            Machine = machine;
        }

        public HASMSource(HASMMachine machine, Stream fs, string workingDirectory = null)
        {
            WorkingDirectory = workingDirectory;
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            Source = new string(bytes.Select(p => (char)p).ToArray());
            Machine = machine;
        }

        public HASMSource(HASMMachine machine, string source, string workingDirectory = null)
        {
            WorkingDirectory = workingDirectory;
            Source = source;
            Machine = machine;
        }

        public int UsedFlash
        {
            get
            {
                throw new NotImplementedException();
                // ParseResult.Sum(p => p.FixedSize);
            }
        }

        public Assembly Assembly { get; internal set; }

        public byte[] OutputCompiled()
        {
            /*
            List<byte> bytes = new List<byte>();
            foreach (var item in ParseResult)
            {
                bytes.AddRange(item.ToBytes());
            }
            return bytes.ToArray();
            */
            throw new NotImplementedException();
        }

        public void OutputCompiled(string fileName)
        {
            File.WriteAllBytes(fileName, OutputCompiled());
        }
    }
}