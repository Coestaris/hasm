﻿using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using HASMLib.Parser.SyntaxTokens;

namespace HASMLib
{
    public class HASMSource
    {
        internal List<SourceLine> _lines;
        internal List<UnknownLabelNameError> _unknownLabelNameErrorList;
        internal List<Variable> _variables;
        internal List<NamedConstant> _namedConsts;
        internal int _constIndex;
        internal int _expressionIndex;
        internal int _varIndex;
        internal int _instructionIndex;

        public string Source { get; set; }
        public string BaseFilename { get; set; }
        public string WorkingDirectory { get; set; }
        public HASMMachine Machine { get; set; }

        public TimeSpan ParseTime { get; internal set; }
        public List<MemZoneFlashElement> ParseResult { get; internal set; }

        public HASMSource(HASMMachine machine, string fileName, string workingDirectory = null)
        {
            BaseFilename = fileName;
            Machine = machine;
        }

        public HASMSource(HASMMachine machine, Stream fs)
        {
			byte[] bytes = new byte[fs.Length];
			fs.Read (bytes, 0, (int)fs.Length);
			Source = new string(bytes.Select(p => (char)p).ToArray());
			Machine = machine;
        }

		public HASMSource(HASMMachine machine, string source)
        {
			Source = source;
			Machine = machine;
        }
        
		public int UsedFlash 
		{
			get
			{
				return ParseResult.Sum (p => p.FixedSize);
			}
		}

		public byte[] OutputCompiled()
		{
			List<byte> bytes = new List<byte> ();
			foreach (var item in ParseResult) {
				bytes.AddRange (item.ToBytes ());
			}
			return bytes.ToArray ();
		}

		public void OutputCompiled(string fileName)
		{
			File.WriteAllBytes (fileName, OutputCompiled ());
		}
    }
}