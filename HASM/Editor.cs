using FastColoredTextBoxNS;
using HASM.Classes;
using HASMLib;
using HASMLib.Core;
using HASMLib.Core.BaseTypes;
using HASMLib.Parser;
using HASMLib.Parser.SourceParsing;
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
        private List<Integer> Output;
        private List<ParseTask> Tasks;

        public void Run(string FileName)
        {
            HASMMachine machine = new HASMMachine(
                (uint)workingFolder.CompileConfig.RAM, 
                (uint)workingFolder.CompileConfig.EEPROM, 
                (uint)workingFolder.CompileConfig.Flash,
                workingFolder.CompileConfig.Base)
            {
                BannedFeatures = workingFolder.CompileConfig.BannedFeatures,
                UserDefinedDefines = workingFolder.CompileConfig.Defines
                    .FindAll(p => !string.IsNullOrEmpty(p.Name))
                    .Select(p => new HASMLib.Parser.SyntaxTokens.Preprocessor.Define(p.Name, p.Value))
                    .ToList()
            };

            machine.SetRegisters(workingFolder.CompileConfig.RegisterNameFormat, (uint)workingFolder.CompileConfig.RegisterCount);
            
            HASMSource source = new HASMSource(machine, FileName, null);

            loadingCircle1.Visible = true;
            tabControl1.Enabled = false;
            stopToolStripMenuItem.Enabled = true;

            var parser = new ParseTaskRunner(source);
            parser.AsyncParseEnd += ParsingEnd;
            parser.AsyncTaskСhanged += Parser_AsyncTaskChanged;

            parser.RunAsync();
        }

        private void Parser_AsyncTaskChanged(ParseTaskRunner runner, HASMSource Source)
        {
            if (InvokeRequired)
            {
                Invoke(new ParseTaskRunner.AsyncTaskChangedDelegate(Parser_AsyncTaskChanged), runner, Source);
            }
            else
            {
                Tasks = runner.Tasks;
                OutputToTextBox();
            }
        }

        delegate void RunEndDelegate(IOStream iostream, RuntimeMachine runtime, HASMSource source);

        void ParsingEnd(ParseTaskRunner runner, HASMSource source)
        {
            if (InvokeRequired)
            {
                var del = new ParseTaskRunner.AsyncParseEndDelegate(ParsingEnd);
                Invoke(del, runner, source);
            }
            else
            {
                if (runner.Status == ParseTaskStatus.Failed)
                {
                    ParseError error = runner.Tasks[runner.FailedTaskIndex].Error;

                    error.Line++;
                    MessageBox.Show($"Task \"{runner.Tasks[runner.FailedTaskIndex].Name}\" failed\n{error.ToString()}", "Parsing error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    loadingCircle1.Visible = false;
                    stopToolStripMenuItem.Enabled = false;
                    tabControl1.Enabled = true;

                    if (error.FileName != null)
                    {
                        bool found = false;
                        foreach (TextEditor page in tabControl1.TabPages)
                            if (page.Path == error.FileName)
                            {
                                tabControl1.SelectedTab = page;
                                found = true;
                                break;
                            }

                        if (!found) AddTab(error.FileName);

                        if (error.Line != -1)
                        {
                            TextEditor tab = (tabControl1.SelectedTab as TextEditor);
                            FastColoredTextBox tb = tab.TextBox;
                            var minLines = 0;
                            var maxLines = tb.LinesCount;
                            var max = tb.VerticalScroll.Maximum;
                            var min = tb.VerticalScroll.Minimum;
                            var currentLine = error.Line;
                            var maxLinesInScreen = tb.Height / tb.Font.SizeInPoints;

                            if (tab.HighlightedLine != -1)
                            {
                                tb[tab.HighlightedLine].BackgroundBrush = Brushes.Transparent;
                                tab.HighlightedLine = -1;
                            };

                            tab.HighlightedLine = error.Line - 1;
                            tb[error.Line - 1].BackgroundBrush = Brushes.Pink;

                            if (maxLinesInScreen < maxLines)
                            {
                                currentLine = Math.Max(Math.Abs((int)(maxLinesInScreen / 2) - currentLine) + 1, 1);
                                int position = (int)((currentLine - minLines) * (max - min) / (float)(maxLines - minLines) + min);
                                tb.VerticalScroll.Value = position - 1;
                                tb.VerticalScroll.Value = position;
                            }

                        }
                    }
                }
                else
                {
                    IOStream stdOut = new IOStream("stdout", StreamDirection.Out);
                    IOStream stdIn = new IOStream("stdin", StreamDirection.In);

                    var runtime = source.Machine.CreateRuntimeMachine(source, new List<IOStream>()
                    {
                        stdOut,
                        stdIn
                    });

                    runThread = new Thread(p =>
                    {
                        var result = runtime.Run();
                        if (result != null)
                            MessageBox.Show(result.ToString(), "Runtime error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        RunEnd(stdOut, runtime, source);
                    });

                    runThread.Start();
                }
            }
        }

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
                    int size = 0;//source.ParseResult.Sum(p => p.FixedSize);
                    toolStripLabel1.Text =
                        $"Parsed in: {Formatter.ToPrettyFormat(source.ParseTime)}" +
                        $" | Byte code size: {size} {(HASMBase.IsSTD ? "byte" : "fbn")}{(size == 1 ? "" : "s")}" +
                        $" | Run in: {Formatter.ToPrettyFormat(runtime.TimeOfRunning)} or {runtime.Ticks} step{(runtime.Ticks == 1 ? "" : "s")}" +
                        $" | Result is: {Output.Count} {(HASMBase.IsSTD ? "byte" : "fbn")}{(Output.Count == 1 ? "" : "s")}";
                }


                OutputToTextBox();
                loadingCircle1.Visible = false;
                stopToolStripMenuItem.Enabled = false;
                tabControl1.Enabled = true;

                (tabControl1.SelectedTab as TextEditor)?.TextBox.Focus();
            }
        }

        void OutputToTextBox()
        {
            if(workingFolder.UserConfig.OutputBuildLog)
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                Tasks.ForEach(p => timeSpan += p.Length);

                richTextBox1.Text =
                    $" ==== Build started at {Tasks[0].StartTime} ====\n" +
                    string.Join("\n", Tasks.Select((p) =>
                    {
                        string str = $"{p.Name} - {p.Status}";
                        if (p.Status != ParseTaskStatus.Waiting)
                            str += $" - {Formatter.ToPrettyFormat(p.Length)}";

                        if (p.Status == ParseTaskStatus.Failed)
                            str += $"\n\nParse Error: {p.Error}\n";

                        return str;
                    })) +
                    $"\n ==== Time spent: {Formatter.ToPrettyFormat(timeSpan)} ====";
            }
            else if (Output != null) switch (outputType)
            {
                case OutputType.Hex:
                    richTextBox1.Text = string.Join(", ", Output.Select(p => "0x" + p.ToString("X")));
                    break;

                case OutputType.Bin:
                    richTextBox1.Text = string.Join(", ", Output.Select(p =>
                    {
                        string baseStr = Convert.ToString((long)p, 2);
                        baseStr = new string('0', (HASMBase.IsSTD ? 8 : BaseIntegerType.PrimitiveType.Base) - baseStr.Length) + baseStr;
                        return "0b" + baseStr;

                    }));
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

                    Directory.CreateDirectory(folderBrowserDialog1.SelectedPath + WorkingFolder.IdeDirPostfix);

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
                splitContainer_main.SplitterDistance = workingFolder.UserConfig.MainSplitterDistance;
                splitContainer_editor.SplitterDistance = workingFolder.UserConfig.EditorSplitterDistance;

                if (workingFolder.UserConfig.WindowPosition.X == -32000)
                {
                    WindowState = FormWindowState.Minimized;
                }
                else
                {
                    Left = workingFolder.UserConfig.WindowPosition.X;
                    Top = workingFolder.UserConfig.WindowPosition.Y;
                }
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
            if (e.Button == MouseButtons.Left)
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
            else
            {
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    Rectangle r = tabControl1.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        toolStripMenuItem_name.Text = (tabControl1.TabPages[i] as TextEditor).DisplayName;
                        contextMenuStrip_tab.Show(PointToScreen(e.Location));
                    }
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

            if (fnode.isRoot) toolStripMenuItem_node_name.Text = "[root]";
            else toolStripMenuItem_node_name.Text = 
                    $"{(fnode.IsDir ? "[" : "")}" +
                    $"{Formatter.MakeRelative(fnode.AbsolutePath, workingFolder.Path + "\\")}" +
                    $"{(fnode.IsDir ? "]" : "")}";

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
            workingFolder.Save();
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

            string newPath = destinationNode.AbsolutePath + '\\' + Path.GetFileName(sourceNode.AbsolutePath);

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
            binOutputToolStripMenuItem.Checked = false;

            OutputToTextBox();
            workingFolder.UserConfig.OutputType = outputType;
            workingFolder.SaveUser();
        }

        private void decOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Dec;
            hexOutputToolStripMenuItem.Checked = false;
            charOutputToolStripMenuItem.Checked = false;
            binOutputToolStripMenuItem.Checked = false;
            
            OutputToTextBox();
            workingFolder.UserConfig.OutputType = outputType;
            workingFolder.SaveUser();
        }

        private void charOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Char;
            decOutputToolStripMenuItem.Checked = false;
            hexOutputToolStripMenuItem.Checked = false;
            binOutputToolStripMenuItem.Checked = false;

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

        private void binOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputType = OutputType.Bin;
            binOutputToolStripMenuItem.Checked = true;
            charOutputToolStripMenuItem.Checked = false;
            decOutputToolStripMenuItem.Checked = false;
            hexOutputToolStripMenuItem.Checked = false;

            OutputToTextBox();
            workingFolder.UserConfig.OutputType = outputType;
            workingFolder.SaveUser();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tab = (tabControl1.SelectedTab as TextEditor);

            Run(tab.Path);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            var tab = (tabControl1.SelectedTab as TextEditor);

            tab.Save();
        }

        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var tab = (tabControl1.SelectedTab as TextEditor);

            tab.Close();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TextEditor tab in tabControl1.TabPages)
            {
                tab.Close();
            }
        }

        private void closeAllExceptThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currentTab = (tabControl1.SelectedTab as TextEditor);
            foreach (TextEditor tab in tabControl1.TabPages)
            {
                if (tab != currentTab) tab.Close();
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            foreach (TextEditor tab in tabControl1.TabPages)
            {
                tab.Save();
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            var tab = (tabControl1.SelectedTab as TextEditor);
            if (tab != null)
            {
                tab.Save();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem_output.Checked = false;
            toolStripMenuItem_buildLog.Checked = true;
            workingFolder.UserConfig.OutputBuildLog = true;

            workingFolder.SaveUser();

            OutputToTextBox();
        }

        private void outputToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem_output.Checked = true;
            toolStripMenuItem_buildLog.Checked = false;
            workingFolder.UserConfig.OutputBuildLog = false;

            workingFolder.SaveUser();

            OutputToTextBox();
        }
    }
}
