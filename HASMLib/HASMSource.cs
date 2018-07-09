﻿using HASMLib.Core.MemoryZone;
using HASMLib.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace HASMLib
{
    public class HASMSource
    {
        public TimeSpan ParseTime { get; private set; }
        public string Source { get; set; }
        public string BaseFilename { get; set; }
        public string WorkingDirectory { get; set; }
		public List<MemZoneFlashElement> ParseResult { get; private set; }

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


		public HASMMachine Machine { get ; set; }

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

		public ParseError Parse()
		{
            DateTime startTime = DateTime.Now;

            ParseResult = new HASMParser().Parse(Machine, out ParseError err, BaseFilename, WorkingDirectory, Machine.UserDefinedDefines);

            if (err != null)
				return err;

			if (ParseResult == null)
				return new ParseError (ParseErrorType.Other_UnknownError);

            ParseTime = TimeSpan.FromMilliseconds((DateTime.Now - startTime).TotalMilliseconds);

            return null;
		}
    }
}