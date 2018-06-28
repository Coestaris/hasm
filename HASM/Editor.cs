using HASMLib;
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

            Text = $"HASM Editor { (tabControl1.SelectedTab == null ? "" : " - " + (tabControl1.SelectedTab as TextEditor).Path)}";
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

            Point pnt = PointToScreen(e.Location);
            pnt.Y += menuStrip1.Height;

            contextMenuStrip_node.Show(pnt);
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
                        if (!page.Close())
                            return;

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
                var dialog = new EnterNameDialog(selectedNode.IsDir ? selectedNode.AbsolutePath : workingFolder.Path)
                {
                    Text = "Enter name of new dir"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
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
                var dialog = new EnterNameDialog(selectedNode.IsDir ? selectedNode.AbsolutePath : workingFolder.Path)
                {
                    Text = "Enter name of new file"
                };

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
            var dialog = new EnterNameDialog(workingFolder.Path)
            {
                Text = "Enter name of new file"
            };

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
            HASMMachine machine = new HASMMachine((uint)compileConfig.RAM, (uint)compileConfig.EEPROM, (uint)compileConfig.Flash)
            {
                BannedFeatures = compileConfig.BannedFeatures
            };

            machine.SetRegisters(compileConfig.RegisterNameFormat, (uint)compileConfig.RegisterCount);


            HASMParser parser = new HASMParser();

            FileStream fs = File.OpenRead(FileName);
            HASMSource source = new HASMSource(machine, fs);
            fs.Close();

            ParseError error = source.Parse();

            if(error != null)
            {
                error.Line++;
                MessageBox.Show(error.ToString());
                return;
            }

            IOStream iostream = new IOStream();
            var runtime = machine.CreateRuntimeMachine(source, iostream);

            runtime.Run();

            var output = iostream.ReadAll();
            richTextBox1.Text = string.Join(", ", output.Select(p => p.ToString("X")));

            toolStripLabel1.Text = $"Parsed in: {Formatter.ToPrettyFormat(source.ParseTime)}. Run in: {Formatter.ToPrettyFormat(runtime.TimeOfRunning)} or {runtime.Ticks} step{(runtime.Ticks == 1 ? "" : "s")}. Result is: {output.Count} TBN{(output.Count == 1 ? "" : "s")}";
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = (SourceFile)toolStripComboBox1.SelectedItem;
            if(!File.Exists(selected.Path))
            {
                MessageBox.Show("File not found!");
                return;
            }

            foreach (TextEditor item in tabControl1.TabPages)
            {
                item.Save();
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

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            workingFolder.PreferedToCompile = (SourceFile)toolStripComboBox1.SelectedItem;
            workingFolder.Save();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Text = $"HASM Editor { (tabControl1.SelectedTab == null ? "" : " - " + (tabControl1.SelectedTab as TextEditor).Path)}";
            workingFolder.SelectedTab = tabControl1.SelectedIndex;
            workingFolder.Save();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (selectedNode != null && !selectedNode.isRoot)
            {
                var dialog = new EnterNameDialog(selectedNode.AbsolutePath)
                {
                    Text = $"Enter new {(selectedNode.IsDir ? "directory" : "file")} name"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (selectedNode.IsDir)
                            Directory.Move(selectedNode.AbsolutePath, dialog.Value);
                        else
                        {
                            
                            foreach(TextEditor editor in tabControl1.TabPages)
                                if(editor.Path == selectedNode.AbsolutePath)
                                {
                                    if (!editor.Close())
                                        return;

                                    UpdateOpenedTabs();
                                }

                            File.Move(selectedNode.AbsolutePath, dialog.Value);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Unable to rename");
                        return;
                    }

                    workingFolder.SetTreeView(treeView1);
                    treeView1.ExpandAll();
                }
            }
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            FileNode destinationNode = ((TreeView)sender).GetNodeAt(pt) as FileNode;
            FileNode sourceNode = (FileNode)e.Data.GetData(typeof(FileNode));

            if (destinationNode == null)
                destinationNode = ((TreeView)sender).Nodes[0] as FileNode; //root node

            if (!destinationNode.IsDir)
            {
                MessageBox.Show("You can move files/dirs only in directories");
                return;
            }

            string newPath = destinationNode.AbsolutePath + '/' + Path.GetFileName(sourceNode.AbsolutePath);

            if(!sourceNode.IsDir)
            {
                if(File.Exists(newPath))
                {
                    MessageBox.Show("File already exists");
                    return;
                }

                foreach (TextEditor editor in tabControl1.TabPages)
                    if (editor.Path == sourceNode.AbsolutePath)
                    {
                        if (!editor.Close())
                            return;

                        UpdateOpenedTabs();
                    }

                try
                {
                    File.Move(sourceNode.AbsolutePath, newPath);
                }
                catch
                {
                    MessageBox.Show("Unable to rename file");
                    return;
                }

                workingFolder.SetTreeView(treeView1);
                treeView1.ExpandAll();
            }
            else
            {
                if (Directory.Exists(newPath))
                {
                    MessageBox.Show("Directory already exists");
                    return;
                }

                try
                {
                    Directory.Move(sourceNode.AbsolutePath, newPath);
                }
                catch
                {
                    MessageBox.Show("Unable to rename dir");
                    return;
                }

                workingFolder.SetTreeView(treeView1);
                treeView1.ExpandAll();
            }
        }
    }
}
