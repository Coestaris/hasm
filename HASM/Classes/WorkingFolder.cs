using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HASM
{
    [Serializable]
    public class WorkingFolder : ICloneable
    {
        [XmlIgnore]
        public List<SourceFile> SourceFiles;
        
        public List<string> OpenedTabs;

        public string CompileConfigPath;

        [XmlIgnore]
        public string Path;

        public SourceFile PreferedToCompile;

        public void Save()
        {
            ToFile(Path + "/_ide/.cfg", this);
        }

        public static string MakeRelative(string filePath, string referencePath)
        {
            var fileUri = new Uri(filePath);
            var referenceUri = new Uri(referencePath);
            return referenceUri.MakeRelativeUri(fileUri).ToString();
        }

        public object Clone()
        {
            return new WorkingFolder()
            {
                CompileConfigPath = CompileConfigPath,
                OpenedTabs = OpenedTabs.Select(p => new string(p.ToCharArray())).ToList(),
                Path = Path,
                PreferedToCompile = (SourceFile)PreferedToCompile.Clone(),
                SelectedTab = SelectedTab
            };
        }

        public static void ToFile(string filename, WorkingFolder cfg)
        {
            XmlSerializer ser = new XmlSerializer(typeof(WorkingFolder));

            WorkingFolder config = (WorkingFolder)cfg.Clone();

            for (int i = 0; i < config.OpenedTabs.Count; i++)
                config.OpenedTabs[i] = MakeRelative(config.OpenedTabs[i], config.Path + "\\");

            config.PreferedToCompile.Path = MakeRelative(config.PreferedToCompile.Path, config.Path + "\\");
            config.CompileConfigPath = MakeRelative(config.CompileConfigPath, config.Path + "\\");


            FileStream fs = new FileStream(filename, FileMode.Create);
            XmlTextWriter writer = new XmlTextWriter(fs, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 4
            };

            ser.Serialize(writer, config);

            fs.Close();
        }
        

        public static WorkingFolder FromFile(string filename, string directory)
        {
            XmlSerializer ser = new XmlSerializer(typeof(WorkingFolder));

            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            WorkingFolder cfg = (WorkingFolder)ser.Deserialize(reader);
            cfg.Path = new DirectoryInfo(directory).Parent.Parent.FullName;


            if(cfg.OpenedTabs != null) 
                for (int i = 0; i < cfg.OpenedTabs.Count; i++)
                {
                    cfg.OpenedTabs[i] = System.IO.Path.Combine(cfg.Path, cfg.OpenedTabs[i]);
                }

            cfg.PreferedToCompile.Path = System.IO.Path.Combine(cfg.Path, cfg.PreferedToCompile.Path);
            cfg.CompileConfigPath = System.IO.Path.Combine(cfg.Path, cfg.CompileConfigPath);


            fs.Close();
            return cfg;
        }


        [XmlIgnore]
        private ImageList il = null;

        [XmlIgnore]
        private int _imgIndex = 0;

        public int SelectedTab;

        public void SetTreeView(TreeView tv)
        {
            string removeFunc(string s) => s.Remove(0, s.Replace('\\', '/').LastIndexOf('/') + 1);

            SourceFiles = new List<SourceFile>();

            tv.Nodes.Clear();

            il = new ImageList();
            il.Images.Add("dir", new Bitmap("dirIcon.png"));
            il.Images.Add("idedir", new Bitmap("ideDirIcon.png"));

            _imgIndex = 1;
            TreeNode parent = new FileNode(removeFunc(Path), Path, true)
            {
                isRoot = true
            };

            AddNodes(removeFunc, Path, parent);
                
            tv.Nodes.Add(parent);
            tv.ImageList = il;
        }

        private void AddNodes(Func<string, string> removeFunc, string path, TreeNode parent)
        {
            foreach(var dir in Directory.GetDirectories(path))
            {
                TreeNode tn = new FileNode(removeFunc(dir), dir, true)
                {
                    ImageKey = removeFunc(dir) == "_ide" ? "idedir" : "dir",
                    SelectedImageKey = removeFunc(dir) == "_ide" ? "idedir" : "dir"
                };

                parent.Nodes.Add(tn);

                AddNodes(removeFunc, dir, tn);
            }

            foreach(var file in Directory.GetFiles(path))
            {
                var ext = System.IO.Path.GetExtension(file);
                if (!string.IsNullOrEmpty(ext))
                {
                    _imgIndex++; 
                    il.Images.Add(ext, IconManager.FindIconForFilename(file, true));
                    parent.Nodes.Add(new FileNode(removeFunc(file), file, false)
                    {
                        ImageIndex = _imgIndex,
                        SelectedImageIndex = _imgIndex
                    });
                } else
                {
                    parent.Nodes.Add(new FileNode(removeFunc(file), file, false));
                }

                SourceFiles.Add(new SourceFile(removeFunc(file), file));
            }
        }
    }
}
