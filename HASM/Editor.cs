using HASMLib;
using HASMLib.Core;
using HASMLib.Parser;
using HASMLib.Runtime;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HASM
{
    public partial class Editor : Form
    {
        public Editor()
        {
            InitializeComponent();
        }

        private WorkingFolder workingFolder;

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1_SizeChanged(null, null);

            string configName = "C:/Users/Notebook/Desktop/hasmProject/_ide/.cfg";
            workingFolder = WorkingFolder.FromFile(configName);

            if(workingFolder.OpenedTabs != null)
            {
                foreach(string path in workingFolder.OpenedTabs)
                {
                    AddTab(path);
                }
            }

            if (File.Exists(workingFolder.ConfigPath))
            {
                compileConfig = CompileConfig.FromFile(workingFolder.ConfigPath);
                compileConfig.FileName = workingFolder.ConfigPath;
            }
            else
            {
                compileConfig = new CompileConfig()
                {
                    FileName = workingFolder.ConfigPath
                };
            }

            workingFolder.SetTreeView(treeView1);
            treeView1.ExpandAll();

            toolStripComboBox1.Items.Clear();
            toolStripComboBox1.Items.AddRange(workingFolder.SourceFiles.Select(p => (object)p).ToArray());

            toolStripComboBox1.SelectedItem = workingFolder.PreferedToCompile;

            tabControl1.SelectedIndex = workingFolder.SelectedTab;
        }

        private void AddTab(string path)
        {
            foreach (TextEditor page in tabControl1.TabPages)
            {
                if (page.Path == path)
                {
                    tabControl1.SelectedTab = page;
                    return;
                }
            }

            var editor = new TextEditor(path);

            tabControl1.TabPages.Add(editor);
            tabControl1.SelectedTab = editor;

            UpdateOpenedTabs();
        }

        private void AddTab(FileNode node)
        {
            AddTab(node.AbsolutePath);
        }

        private void UpdateOpenedTabs()
        {
            workingFolder.OpenedTabs = new List<string>();
            foreach (TextEditor item in tabControl1.TabPages)
                workingFolder.OpenedTabs.Add(item.Path);
            workingFolder.Save();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if ((e.Node as FileNode).IsDir)
                return;

            AddTab(e.Node as FileNode);
            UpdateOpenedTabs();
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                Rectangle r = tabControl1.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                if (closeButton.Contains(e.Location))
                {
                    (tabControl1.TabPages[i] as TextEditor).Close();
                    UpdateOpenedTabs();
                }
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            treeView1.Height = Height - 94;
            tabControl1.Width = Width - 219;
            tabControl1.Height = Height - 194;

            panel1.Width = Width - 219;
            panel1.Top = Height - 161;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            FileNode fnode = e.Node as FileNode;

            contextMenuStrip_node.Show(PointToScreen(e.Location));
            selectedNode = fnode;
        }

        private FileNode selectedNode = null;
        private CompileConfig compileConfig;

        private void contextMenuStrip_node_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            //selectedNode = null;
        }

        private void openToolStripMenuItem_open_Click(object sender, EventArgs e)
        {
            if (selectedNode != null && !selectedNode.IsDir)
            {
                AddTab(selectedNode);
                UpdateOpenedTabs();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(selectedNode != null && MessageBox.Show($"Are you sure you want to delete {selectedNode.AbsolutePath}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (TextEditor page in tabControl1.TabPages)
                {
                    if (page.Path == selectedNode.AbsolutePath)
                    {
                        page.Close();
                        UpdateOpenedTabs();
                    }
                }

                if (selectedNode.IsDir)
                    Directory.Delete(selectedNode.AbsolutePath, true);
                else
                    File.Delete(selectedNode.AbsolutePath);

                workingFolder.SetTreeView(treeView1);
                treeView1.ExpandAll();
            }
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                workingFolder.SetTreeView(treeView1);
                treeView1.ExpandAll();
            }
        }

        private void folderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                var dialog = new EnterNameDialog(selectedNode.IsDir ? selectedNode.AbsolutePath : workingFolder.Path);
                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Directory.CreateDirectory(dialog.Value);
                    } catch
                    {
                        MessageBox.Show("Unable to create dir");
                        return;
                    }

                    workingFolder.SetTreeView(treeView1);
                    treeView1.ExpandAll();
                }
            }
        }

        private void sourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                var dialog = new EnterNameDialog(selectedNode.IsDir ? selectedNode.AbsolutePath : workingFolder.Path);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Create(dialog.Value).Close();
                    } catch
                    {
                        MessageBox.Show("Unable to create file");
                        return;
                    }

                    workingFolder.SetTreeView(treeView1);
                    treeView1.ExpandAll();
                }
            }
        }

        private void addSourceFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new EnterNameDialog(workingFolder.Path);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Create(dialog.Value).Close();
                }
                catch
                {
                    MessageBox.Show("Unable to create file");
                    return;
                }

                workingFolder.SetTreeView(treeView1);
                treeView1.ExpandAll();
            }
        }

        private void runDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lastSelected = toolStripComboBox1.SelectedItem;
            
            toolStripComboBox1.Items.Clear();
            toolStripComboBox1.Items.AddRange(workingFolder.SourceFiles.Select(p => (object)p).ToArray());

            if (lastSelected != null)
                toolStripComboBox1.SelectedItem = lastSelected;
        }

        private void Run(string FileName)
        {
           // HASMMachine machine = new HASMMachine();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = (SourceFile)toolStripComboBox1.SelectedItem;
            if(!File.Exists(selected.Path))
            {
                MessageBox.Show("File not found!");
                return;
            }

            Run(selected.Path);
        }

        private void compileOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new CompileOptions(compileConfig);
            
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                compileConfig = dialog.config;
            }
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            workingFolder.PreferedToCompile = (SourceFile)toolStripComboBox1.SelectedItem;
            workingFolder.Save();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            workingFolder.SelectedTab = tabControl1.SelectedIndex;
        }
    }
}
