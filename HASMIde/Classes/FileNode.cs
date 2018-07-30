using System.Windows.Forms;

namespace HASM
{
    public class FileNode : TreeNode
    {
        public bool isRoot;

        public string AbsolutePath;
        public bool IsDir;

        public FileNode(string label, string absolutePath, bool isDir)
        {
            Text = label;
            AbsolutePath = absolutePath;
            IsDir = isDir;
        }
    }
}
