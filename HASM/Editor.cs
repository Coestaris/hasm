using HASMLib;
using HASMLib.Core;
using HASMLib.Parser;
using HASMLib.Runtime;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HASM
{
    public partial class Editor : Form
    {
        public Editor()
        {
            InitializeComponent();
        }

        public static Editor Self;

        private WorkingFolder workingFolder;

        private void Form1_Load(object sender, EventArgs e)
        {
            Self = this;

            tabControl1_SizeChanged(null, null);
            string configName = "";

            if(File.Exists("directory.txt"))
                configName = File.ReadAllText("directory.txt");
            
            if (!Directory.Exists(configName))
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    configName = folderBrowserDialog1.SelectedPath + "/_ide/.cfg";
                    string compileConfigFileName = folderBrowserDialog1.SelectedPath + "/_ide/compile.cfg";

                    Directory.CreateDirectory(folderBrowserDialog1.SelectedPath + "/_ide");

                    workingFolder = new WorkingFolder()
                    {
                        CompileConfigPath = compileConfigFileName,
                        Path = folderBrowserDialog1.SelectedPath,
                    };

                    WorkingFolder.ToFile(configName, workingFolder);
                    File.WriteAllText("directory.txt", folderBrowserDialog1.SelectedPath);
                }
                else
                {
                    Close();
                    return;
                }
            }
            else
            {
                configName += "/_ide/.cfg";
                if (File.Exists(configName))
                {
                    workingFolder = WorkingFolder.FromFile(configName, configName);
                } else
                {
                    workingFolder = new WorkingFolder()
                    {
                        CompileConfigPath = new FileInfo(configName).DirectoryName + "/compile.cfg",
                        Path = new FileInfo(configName).Directory.Parent.FullName,
                    };

                    WorkingFolder.ToFile(configName, workingFolder);
                }
            }

            if(workingFolder.OpenedTabs != null)
            {
                foreach(string path in workingFolder.OpenedTabs)
                {
                    AddTab(path);
                }
            }

            if (File.Exists(workingFolder.CompileConfigPath))
            {
                compileConfig = CompileConfig.FromFile(workingFolder.CompileConfigPath);
                compileConfig.FileName =  workingFolder.CompileConfigPath;
            }
            else
            {
                compileConfig = new CompileConfig()
                {
                    FileName = workingFolder.CompileConfigPath
                };
                CompileConfig.ToFile(workingFolder.CompileConfigPath, compileConfig);
            }

            workingFolder.SetTreeView(treeView1);
            treeView1.ExpandAll();

            toolStripComboBox1.Items.Clear();
            toolStripComboBox1.Items.AddRange(workingFolder.SourceFiles.Select(p => (object)p).ToArray());

            toolStripComboBox1.SelectedItem = workingFolder.PreferedToCompile;

            tabControl1.SelectedIndex = workingFolder.SelectedTab;

            switch (workingFolder.OutputType)
            {
                case OutputType.Hex:
                    hexOutputToolStripMenuItem.Checked = true;
                    break;
                case OutputType.Dec:
                    decOutputToolStripMenuItem.Checked = true;
                    break;
                case OutputType.Char:
                    charOutputToolStripMenuItem.Checked = true;
                    break;
                default:
                    break;
            }
            outputType = workingFolder.OutputType;

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

        public void Run(string FileName)
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

            loadingCircle1.Visible = true;
            splitContainer_editor.Enabled = false;
            stopToolStripMenuItem.Enabled = true;

            ParseError error = source.Parse();

            if(error != null)
            {
                error.Line++;
                MessageBox.Show(error.ToString(), "Parsing error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                loadingCircle1.Visible = false;
                stopToolStripMenuItem.Enabled = false;
                splitContainer_editor.Enabled = true;
                return;
            }

            IOStream iostream = new IOStream();
            var runtime = machine.CreateRuntimeMachine(source, iostream);

            runThread = new Thread(p =>
            {
                var result = runtime.Run();
                if (result != RuntimeOutputCode.OK)
                    MessageBox.Show($"Runtime error: {result}", "Runtime error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                RunEnd(iostream, runtime, source);
            });


            runThread.Start();
        }

        Thread runThread;

        delegate void RunEndDelegate(IOStream iostream, RuntimeMachine runtime, HASMSource source);

        void RunEnd(IOStream iostream, RuntimeMachine runtime, HASMSource source)
        {
            if(InvokeRequired)
            {
                Invoke(new RunEndDelegate(RunEnd), iostream, runtime, source);
            }
            else
            {
                if (runtime == null || source == null)
                {
                    toolStripLabel1.Text = "Run aborted";
                    Output = null;
                }
                else
                {
                    Output = iostream.ReadAll();
                    int size = source.ParseResult.Sum(p => p.FixedSize);
                    toolStripLabel1.Text =
                        $"Parsed in: {Formatter.ToPrettyFormat(source.ParseTime)}. " +
                        $"Parsed size: {size}TBN{(size == 1 ? "" : "s")}. " +
                        $"Run in: {Formatter.ToPrettyFormat(runtime.TimeOfRunning)} or {runtime.Ticks} step{(runtime.Ticks == 1 ? "" : "s")}. " +
                        $"Result is: {Output.Count} TBN{(Output.Count == 1 ? "" : "s")}";
                }


                OutputToTextBox();
                loadingCircle1.Visible = false;
                stopToolStripMenuItem.Enabled = false;
                splitContainer_editor.Enabled = true;

                (tabControl1.SelectedTab as TextEditor)?.TextBox.Focus();
            }
        }

        void OutputToTextBox()
        {
            if(Output != null) switch (outputType)
            {
                case OutputType.Hex:
                    richTextBox1.Text = string.Join(", ", Output.Select(p => "0x" + p.ToString("X")));
                    break;

                case OutputType.Dec:
                    richTextBox1.Text = string.Join(", ", Output.Select(p => p.ToString()));
                    break;

                case OutputType.Char:
                    richTextBox1.Text = string.Join("", Output.Select(p => (char)p));
                    break;

                default:
                    break;
            }
        }

        List<UInt12> Output;

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

        public enum OutputType
        {
            Hex,
            Dec,
            Char
        }

        private OutputType outputType;

        private void hexOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Hex;
            decOutputToolStripMenuItem.Checked = false;
            charOutputToolStripMenuItem.Checked = false;

            OutputToTextBox();
            workingFolder.OutputType = outputType;
            workingFolder.Save();
        }

        private void decOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Dec;
            hexOutputToolStripMenuItem.Checked = false;
            charOutputToolStripMenuItem.Checked = false;

            OutputToTextBox();
            workingFolder.OutputType = outputType;
            workingFolder.Save();
        }

        private void charOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Char;
            decOutputToolStripMenuItem.Checked = false;
            hexOutputToolStripMenuItem.Checked = false;

            OutputToTextBox();
            workingFolder.OutputType = outputType;
            workingFolder.Save();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runThread.Abort();
            RunEnd(null, null, null);
        }

        private void tabControl1_SizeChanged(object sender, EventArgs e)
        {
            loadingCircle1.Top =  Height / 2 - loadingCircle1.Height / 2;
            loadingCircle1.Left = Width / 2 - loadingCircle1.Width / 2;
        }
    }
}
