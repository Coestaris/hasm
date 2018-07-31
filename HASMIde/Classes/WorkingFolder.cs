using HASM.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HASM
{
    [Serializable]
    public class WorkingFolder : ICloneable
    {
        public const string IdeDirPostfix = "\\_ide";
        public const string MainConfigPostfix = IdeDirPostfix + "\\.cfg";
        public const string CompileConfigPostfix = IdeDirPostfix + "\\compile.cfg";
        public const string UserConfigPostfix = IdeDirPostfix + "\\user.cfg";

        [XmlIgnore]
        public List<SourceFile> SourceFiles;
        public string CompileConfigPath;

        [XmlIgnore]
        public string Path;

        [XmlIgnore]
        public UserConfig UserConfig;

        [XmlIgnore]
        public CompileConfig CompileConfig;

        public string UserConfigPath;

        public SourceFile PreferedToCompile;
        
        public void Save()
        {
            ToFile(Path + MainConfigPostfix, this);
        }

        public void SaveUser()
        {
            UserConfig.ToFile(UserConfigPath, UserConfig);
        }

        public object Clone()
        {
            return new WorkingFolder()
            {
                CompileConfigPath = CompileConfigPath,
                UserConfigPath = UserConfigPath,
                Path = Path,
                PreferedToCompile = (SourceFile)PreferedToCompile.Clone(),
            };
        }

        public static void ToFile(string filename, WorkingFolder cfg)
        {
            XmlSerializer ser = new XmlSerializer(typeof(WorkingFolder));

            WorkingFolder config = (WorkingFolder)cfg.Clone();

            if(!string.IsNullOrEmpty(config.PreferedToCompile.Path))
                config.PreferedToCompile.Path = Formatter.MakeRelative(config.PreferedToCompile.Path, config.Path + "\\");

            if (!string.IsNullOrEmpty(config.CompileConfigPath))
                config.CompileConfigPath = Formatter.MakeRelative(config.CompileConfigPath, config.Path + "\\");

            if (!string.IsNullOrEmpty(config.UserConfigPath))
                config.UserConfigPath = Formatter.MakeRelative(config.UserConfigPath, config.Path + "\\");


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


            cfg.PreferedToCompile.Path = System.IO.Path.Combine(cfg.Path, cfg.PreferedToCompile.Path);
            cfg.CompileConfigPath = System.IO.Path.Combine(cfg.Path, cfg.CompileConfigPath);
            cfg.UserConfigPath = System.IO.Path.Combine(cfg.Path, cfg.UserConfigPath);


            fs.Close();
            return cfg;
        }


        [XmlIgnore]
        private ImageList il = null;

        [XmlIgnore]
        private int _imgIndex = 0;

        public void SetTreeView(TreeView tv)
        {
            string removeFunc(string s) => s.Remove(0, s.Replace('\\', '/').LastIndexOf('/') + 1);

            SourceFiles = new List<SourceFile>();

            tv.Nodes.Clear();

            il = new ImageList();
            il.Images.Add("dir", new Bitmap("icons\\dirIcon.png"));
            il.Images.Add(".cfg", new Bitmap("icons\\cfg.png"));
            il.Images.Add("idedir", new Bitmap("icons\\ideDirIcon.png"));

            _imgIndex = 1;
            TreeNode parent = new FileNode(removeFunc(Path), Path, true)
            {
                isRoot = true
            };

            tv.ImageList = il;
            AddNodes(removeFunc, Path, parent);
            tv.Nodes.Add(parent);
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
                    if(!il.Images.Keys.Contains(ext))
                        il.Images.Add(ext, IconManager.FindIconForFilename(file, true));

                    parent.Nodes.Add(new FileNode(removeFunc(file), file, false)
                    {
                        ImageKey = ext,
                        SelectedImageKey = ext
                    });
                } else
                {
                    parent.Nodes.Add(new FileNode(removeFunc(file), file, false));
                }

                if(file.EndsWith(".hasm"))
                    SourceFiles.Add(new SourceFile(Formatter.MakeRelative(file, Path + "\\"), file));
            }
        }
    }
}
