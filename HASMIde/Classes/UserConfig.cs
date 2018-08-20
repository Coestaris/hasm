using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace HASM.Classes
{
    [Serializable]
    public class UserConfig : ICloneable
    {
        [XmlIgnore]
        public string Path;

        public List<string> OpenedTabs;
        public OutputType OutputType;
        public int SelectedTab;
        public Size WindowSize;
        public Point WindowPosition;
        public int EditorSplitterDistance;
        public int MainSplitterDistance;
        public bool OutputBuildLog;

        public static void ToFile(string filename, UserConfig cfg)
        {
            XmlSerializer ser = new XmlSerializer(typeof(UserConfig));

            UserConfig config = (UserConfig)cfg.Clone();

            for (int i = 0; i < config.OpenedTabs.Count; i++)
				config.OpenedTabs[i] = Formatter.MakeRelative(config.OpenedTabs[i], config.Path + PlatformSpecific.NameSeparator);

            FileStream fs = new FileStream(filename, FileMode.Create);
            XmlTextWriter writer = new XmlTextWriter(fs, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 4
            };

            ser.Serialize(writer, config);

            fs.Close();
        }


        public static UserConfig FromFile(string filename, string directory)
        {
            XmlSerializer ser = new XmlSerializer(typeof(UserConfig));

            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            UserConfig cfg = (UserConfig)ser.Deserialize(reader);
            cfg.Path = directory;
            
            if (cfg.OpenedTabs != null)
                for (int i = 0; i < cfg.OpenedTabs.Count; i++)
                {
				    cfg.OpenedTabs[i] = PlatformSpecific.CombinePath(cfg.Path, cfg.OpenedTabs[i]);
                }

            fs.Close();
            return cfg;
        }

        public object Clone()
        {
            return new UserConfig()
            {
                EditorSplitterDistance = EditorSplitterDistance,
                MainSplitterDistance = MainSplitterDistance,
				OpenedTabs = OpenedTabs.Select(p => (string)p.Clone()).ToList(),
                OutputType = OutputType,
                Path = Path,
                SelectedTab = SelectedTab,
                WindowPosition = WindowPosition,
                WindowSize = WindowSize
            };
        }
    }
}
