using HASM.Classes;
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

        private OutputType outputType;

        private WorkingFolder workingFolder;

        private FileNode selectedNode = null;

        private Thread runThread;

        private List<UInt12> Output;
      
        public void Run(string FileName)
        {
            HASMMachine machine = new HASMMachine((uint)workingFolder.CompileConfig.RAM, (uint)workingFolder.CompileConfig.EEPROM, (uint)workingFolder.CompileConfig.Flash)
            {
                BannedFeatures = workingFolder.CompileConfig.BannedFeatures
            };

            machine.SetRegisters(workingFolder.CompileConfig.RegisterNameFormat, (uint)workingFolder.CompileConfig.RegisterCount);
            HASMParser parser = new HASMParser();

            FileStream fs = File.OpenRead(FileName);
            HASMSource source = new HASMSource(machine, fs);
            fs.Close();

            loadingCircle1.Visible = true;
            splitContainer_editor.Enabled = false;
            stopToolStripMenuItem.Enabled = true;

            ParseError error = source.Parse();

            if (error != null)
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

        delegate void RunEndDelegate(IOStream iostream, RuntimeMachine runtime, HASMSource source);

        void RunEnd(IOStream iostream, RuntimeMachine runtime, HASMSource source)
        {
            if (InvokeRequired)
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
            if (Output != null) switch (outputType)
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
            workingFolder.UserConfig.OpenedTabs = new List<string>();
            foreach (TextEditor item in tabControl1.TabPages)
                workingFolder.UserConfig.OpenedTabs.Add(item.Path);
            workingFolder.SaveUser();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Self = this;

            tabControl1_SizeChanged(null, null);
            string configName = "";

            if (File.Exists("directory.txt"))
                configName = File.ReadAllText("directory.txt");

            if (!Directory.Exists(configName))
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    configName = folderBrowserDialog1.SelectedPath + WorkingFolder.MainConfigPostfix;
                    string compileConfigFileName = folderBrowserDialog1.SelectedPath + WorkingFolder.CompileConfigPostfix;
                    string userConfigFileName = folderBrowserDialog1.SelectedPath + WorkingFolder.UserConfigPostfix;

                    Directory.CreateDirectory(folderBrowserDialog1.SelectedPath + "/_ide");

                    workingFolder = new WorkingFolder()
                    {
                        CompileConfigPath = compileConfigFileName,
                        UserConfigPath = userConfigFileName,
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
                configName += WorkingFolder.MainConfigPostfix;
                if (File.Exists(configName))
                {
                    workingFolder = WorkingFolder.FromFile(configName, configName);
                }
                else
                {
                    workingFolder = new WorkingFolder()
                    {
                        CompileConfigPath = new FileInfo(configName).DirectoryName + WorkingFolder.CompileConfigPostfix,
                        UserConfigPath = new FileInfo(configName).DirectoryName + WorkingFolder.UserConfigPostfix,

                        Path = new FileInfo(configName).Directory.Parent.FullName,
                    };
                    WorkingFolder.ToFile(configName, workingFolder);
                }
            }

            if (File.Exists(workingFolder.UserConfigPath))
            {
                workingFolder.UserConfig = UserConfig.FromFile(workingFolder.UserConfigPath, workingFolder.Path);

                Size = workingFolder.UserConfig.WindowSize;
                Left = workingFolder.UserConfig.WindowPosition.X;
                Top = workingFolder.UserConfig.WindowPosition.Y;
                splitContainer_main.SplitterDistance = workingFolder.UserConfig.MainSplitterDistance;
                splitContainer_editor.SplitterDistance = workingFolder.UserConfig.EditorSplitterDistance;
            }
            else
            {
                workingFolder.UserConfig = new UserConfig()
                {
                    EditorSplitterDistance = splitContainer_editor.SplitterDistance,
                    MainSplitterDistance = splitContainer_main.SplitterDistance,
                    OpenedTabs = new List<string>(),
                    OutputType = OutputType.Hex,
                    SelectedTab = -1,
                    WindowPosition = new Point(Left, Top),
                    WindowSize = Size
                };
                UserConfig.ToFile(workingFolder.UserConfigPath, workingFolder.UserConfig);
            }

            if (workingFolder.UserConfig.OpenedTabs != null)
            {
                foreach (string path in workingFolder.UserConfig.OpenedTabs)
                {
                    AddTab(path);
                }
            }

            if (File.Exists(workingFolder.CompileConfigPath))
            {
                workingFolder.CompileConfig = CompileConfig.FromFile(workingFolder.CompileConfigPath);
                workingFolder.CompileConfig.FileName = workingFolder.CompileConfigPath;
            }
            else
            {
                workingFolder.CompileConfig = new CompileConfig()
                {
                    FileName = workingFolder.CompileConfigPath
                };
                CompileConfig.ToFile(workingFolder.CompileConfigPath, workingFolder.CompileConfig);
            }

            workingFolder.SetTreeView(treeView1);
            treeView1.ExpandAll();

            toolStripComboBox1.Items.Clear();
            toolStripComboBox1.Items.AddRange(workingFolder.SourceFiles.Select(p => (object)p).ToArray());

            toolStripComboBox1.SelectedItem = workingFolder.PreferedToCompile;

            tabControl1.Enabled = true;
            tabControl1.SelectedIndex = workingFolder.UserConfig.SelectedTab;

            switch (workingFolder.UserConfig.OutputType)
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
            outputType = workingFolder.UserConfig.OutputType;

            Text = $"HASM Editor { (tabControl1.SelectedTab == null ? "" : " - " + (tabControl1.SelectedTab as TextEditor).Path)}";
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
            var dialog = new CompileOptions(workingFolder.CompileConfig);
            
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                workingFolder.CompileConfig = dialog.config;
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            workingFolder.PreferedToCompile = (SourceFile)toolStripComboBox1.SelectedItem;
            workingFolder.SaveUser();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.Enabled)
            {
                Text = $"HASM Editor { (tabControl1.SelectedTab == null ? "" : " - " + (tabControl1.SelectedTab as TextEditor).Path)}";
                workingFolder.UserConfig.SelectedTab = tabControl1.SelectedIndex;
                workingFolder.SaveUser();
            }
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

        private void hexOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Hex;
            decOutputToolStripMenuItem.Checked = false;
            charOutputToolStripMenuItem.Checked = false;

            OutputToTextBox();
            workingFolder.UserConfig.OutputType = outputType;
            workingFolder.SaveUser();
        }

        private void decOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Dec;
            hexOutputToolStripMenuItem.Checked = false;
            charOutputToolStripMenuItem.Checked = false;

            OutputToTextBox();
            workingFolder.UserConfig.OutputType = outputType;
            workingFolder.SaveUser();
        }

        private void charOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Char;
            decOutputToolStripMenuItem.Checked = false;
            hexOutputToolStripMenuItem.Checked = false;

            OutputToTextBox();
            workingFolder.UserConfig.OutputType = outputType;
            workingFolder.SaveUser();
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

        private void Editor_ResizeEnd(object sender, EventArgs e)
        {
            if (tabControl1.Enabled)
            {
                workingFolder.UserConfig.WindowSize = Size;
                workingFolder.SaveUser();
            }
        }

        private void splitContainer_main_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (tabControl1.Enabled)
            {
                workingFolder.UserConfig.MainSplitterDistance = splitContainer_main.SplitterDistance;
                workingFolder.SaveUser();
            }
        }

        private void splitContainer_editor_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (tabControl1.Enabled)
            {
                workingFolder.UserConfig.EditorSplitterDistance = splitContainer_editor.SplitterDistance;
                workingFolder.SaveUser();
            }
        }

        private void Editor_LocationChanged(object sender, EventArgs e)
        {
            if (workingFolder != null && workingFolder.UserConfig != null && tabControl1.Enabled)
            {
                workingFolder.UserConfig.WindowPosition = new Point(Left, Top);
                workingFolder.SaveUser();
            }
        }
    }
}
