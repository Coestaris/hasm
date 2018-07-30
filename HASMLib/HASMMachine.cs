using HASMLib.Core;
using HASMLib.Parser.SyntaxTokens.Preprocessor;
using HASMLib.Runtime;
using System;
using System.Collections.Generic;

namespace HASMLib
{
    public class HASMMachine
    {
        public UInt32 RegisterCount { get; set; }
        public UInt32 RAM { get; set; }
        public UInt32 EEPROM { get; set; }
        public UInt32 Flash { get; set; }
        public MemZone MemZone;
        public List<Define> UserDefinedDefines { get; set; }
        public HASMMachineBannedFeatures BannedFeatures { get; set; }

        private string _registerNameFormat;

        public HASMMachine(uint ram, uint eeprom, uint flash, int hasmBase = 8)
        {
            SetBase(hasmBase);

            RAM = ram;
            EEPROM = eeprom;
            Flash = flash;

            MemZone = new MemZone((int)flash, (int)ram, null);
        }

        public void SetRegisters(string NameFormat, uint count)
        {
            _registerNameFormat = NameFormat;
            RegisterCount = count;
        }

        public void ClearRegisters()
        {
            if (MemZone.RAM.Count > RegisterCount)
                MemZone.RAM.RemoveRange(0, (int)RegisterCount);
        }

        public void SetBase(int hasmBase = 8)
        {
            HASMBase.Base = hasmBase;
        }

        public List<string> GetRegisterNames()
        {
            var a = new List<string>();
            for (int i = 0; i < RegisterCount; i++)
                a.Add(string.Format(_registerNameFormat, i));
            return a;
        }

        public RuntimeMachine CreateRuntimeMachine(HASMSource source, List<IOStream> streams = null)
        {
            var rm = new RuntimeMachine(this, source);
            MemZone.Clear();
            //MemZone.Flash = source.ParseResult;

            if (streams != null)
                foreach (var stream in streams)
                    stream.Init(rm);

            return rm;
        }
    }
}
