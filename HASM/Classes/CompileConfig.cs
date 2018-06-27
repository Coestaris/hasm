﻿using HASMLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace HASM
{
    [XmlRoot("CompileConfig", Namespace = "http://CompileConfig", IsNullable = false)]
    public class CompileConfig
    {
        [NonSerialized]
        public string FileName;

        public int RAM;

        public int Flash;

        public int EEPROM;

        public HASMMachineBannedFeatures BannedFeatures;

        public List<Define> Defines;

        public CompileConfig(int rAM, int flash, int eEPROM, HASMMachineBannedFeatures bannedFeatures, List<Define> defines)
        {
            RAM = rAM;
            Flash = flash;
            EEPROM = eEPROM;
            BannedFeatures = bannedFeatures;
            Defines = defines;
        }

        public CompileConfig()
        {

        }

        public static void ToFile(string filename, CompileConfig cfg)
        {
            XmlSerializer ser = new XmlSerializer(typeof(CompileConfig));

            FileStream fs = new FileStream(filename, FileMode.Create);
            XmlWriter writer = XmlWriter.Create(fs);

            ser.Serialize(writer, cfg);

            fs.Close();
        }

        public static CompileConfig FromFile(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(CompileConfig));

            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            CompileConfig cfg = (CompileConfig)ser.Deserialize(reader);

            return cfg;
        }
    }
}