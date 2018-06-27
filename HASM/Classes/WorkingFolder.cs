using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HASM
{
    [Serializable]
    public class WorkingFolder
    {
        [XmlIgnore]
        public List<SourceFile> SourceFiles;
        

        public List<string> OpenedTabs;

        public string ConfigPath;

        public string Path;

        public SourceFile PreferedToCompile;

        public void Save()
        {
            ToFile(Path + "/_ide/.cfg", this);
        }

        public static void ToFile(string filename, WorkingFolder cfg)
        {
            XmlSerializer ser = new XmlSerializer(typeof(WorkingFolder));

            FileStream fs = new FileStream(filename, FileMode.Create);
            XmlWriter writer = XmlWriter.Create(fs);

            ser.Serialize(writer, cfg);

            fs.Close();
        }



        public static WorkingFolder FromFile(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(WorkingFolder));

            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            WorkingFolder cfg = (WorkingFolder)ser.Deserialize(reader);
            fs.Close();
            return cfg;
        }


        [XmlIgnore]
        private ImageList il = null;

        [XmlIgnore]
        private int _imgIndex = 0;
        internal int SelectedTab;

        public void SetTreeView(TreeView tv)
        {
            string removeFunc(string s) => s.Remove(0, s.Replace('\\', '/').LastIndexOf('/') + 1);

            SourceFiles = new List<SourceFile>();

            tv.Nodes.Clear();

            il = new ImageList();
            il.Images.Add("dir", new Bitmap("dirIcon.png"));
            il.Images.Add("idedir", new Bitmap("ideDirIcon.png"));

            _imgIndex = 1;
            TreeNode parent = new FileNode(removeFunc(Path), Path, true);
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
