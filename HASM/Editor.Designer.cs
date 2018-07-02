namespace HASM
{
    partial class Editor
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.openFileDialog_source = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog_binnary = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog_source = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog_binnary = new System.Windows.Forms.SaveFileDialog();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveCompiledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCompiledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSourceFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addHeaderFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.precompileAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hexOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.charOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip_node = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem_open = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.splitContainer_main = new System.Windows.Forms.SplitContainer();
            this.splitContainer_editor = new System.Windows.Forms.SplitContainer();
            this.loadingCircle1 = new MRG.Controls.UI.LoadingCircle();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip_node.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_main)).BeginInit();
            this.splitContainer_main.Panel1.SuspendLayout();
            this.splitContainer_main.Panel2.SuspendLayout();
            this.splitContainer_main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_editor)).BeginInit();
            this.splitContainer_editor.Panel1.SuspendLayout();
            this.splitContainer_editor.Panel2.SuspendLayout();
            this.splitContainer_editor.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog_source
            // 
            this.openFileDialog_source.FileName = "openFileDialog_source";
            // 
            // openFileDialog_binnary
            // 
            this.openFileDialog_binnary.FileName = "openFileDialog_binnary";
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(185, 453);
            this.treeView1.TabIndex = 11;
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Enabled = false;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(643, 344);
            this.tabControl1.TabIndex = 13;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.SizeChanged += new System.EventHandler(this.tabControl1_SizeChanged);
            this.tabControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 477);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(832, 25);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.runDebugToolStripMenuItem,
            this.folderToolStripMenuItem,
            this.outputToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(832, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.openToolStripMenuItem,
            this.toolStripMenuItem2,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(137, 22);
            this.toolStripMenuItem1.Text = "Open folder";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(134, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(137, 22);
            this.toolStripMenuItem2.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(134, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // runDebugToolStripMenuItem
            // 
            this.runDebugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.toolStripComboBox1,
            this.stopToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveCompiledToolStripMenuItem,
            this.loadCompiledToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveOutputToolStripMenuItem,
            this.compileOptionsToolStripMenuItem});
            this.runDebugToolStripMenuItem.Name = "runDebugToolStripMenuItem";
            this.runDebugToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.runDebugToolStripMenuItem.Text = "Run and Debug";
            this.runDebugToolStripMenuItem.Click += new System.EventHandler(this.runDebugToolStripMenuItem_Click);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.stopToolStripMenuItem.Text = "Stop running";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // saveCompiledToolStripMenuItem
            // 
            this.saveCompiledToolStripMenuItem.Name = "saveCompiledToolStripMenuItem";
            this.saveCompiledToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.saveCompiledToolStripMenuItem.Text = "Save compiled";
            // 
            // loadCompiledToolStripMenuItem
            // 
            this.loadCompiledToolStripMenuItem.Name = "loadCompiledToolStripMenuItem";
            this.loadCompiledToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.loadCompiledToolStripMenuItem.Text = "Load compiled";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(178, 6);
            // 
            // saveOutputToolStripMenuItem
            // 
            this.saveOutputToolStripMenuItem.Name = "saveOutputToolStripMenuItem";
            this.saveOutputToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.saveOutputToolStripMenuItem.Text = "Save output";
            // 
            // compileOptionsToolStripMenuItem
            // 
            this.compileOptionsToolStripMenuItem.Name = "compileOptionsToolStripMenuItem";
            this.compileOptionsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.compileOptionsToolStripMenuItem.Text = "Compile options";
            this.compileOptionsToolStripMenuItem.Click += new System.EventHandler(this.compileOptionsToolStripMenuItem_Click);
            // 
            // folderToolStripMenuItem
            // 
            this.folderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSourceFileToolStripMenuItem,
            this.addHeaderFileToolStripMenuItem,
            this.toolStripSeparator4,
            this.precompileAllToolStripMenuItem});
            this.folderToolStripMenuItem.Name = "folderToolStripMenuItem";
            this.folderToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.folderToolStripMenuItem.Text = "Folder";
            // 
            // addSourceFileToolStripMenuItem
            // 
            this.addSourceFileToolStripMenuItem.Name = "addSourceFileToolStripMenuItem";
            this.addSourceFileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.addSourceFileToolStripMenuItem.Text = "Add Source File";
            this.addSourceFileToolStripMenuItem.Click += new System.EventHandler(this.addSourceFileToolStripMenuItem_Click);
            // 
            // addHeaderFileToolStripMenuItem
            // 
            this.addHeaderFileToolStripMenuItem.Name = "addHeaderFileToolStripMenuItem";
            this.addHeaderFileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.addHeaderFileToolStripMenuItem.Text = "Add Header File";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(155, 6);
            // 
            // precompileAllToolStripMenuItem
            // 
            this.precompileAllToolStripMenuItem.Name = "precompileAllToolStripMenuItem";
            this.precompileAllToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.precompileAllToolStripMenuItem.Text = "Precompile All";
            // 
            // outputToolStripMenuItem
            // 
            this.outputToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hexOutputToolStripMenuItem,
            this.decOutputToolStripMenuItem,
            this.charOutputToolStripMenuItem});
            this.outputToolStripMenuItem.Name = "outputToolStripMenuItem";
            this.outputToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.outputToolStripMenuItem.Text = "Output";
            // 
            // hexOutputToolStripMenuItem
            // 
            this.hexOutputToolStripMenuItem.CheckOnClick = true;
            this.hexOutputToolStripMenuItem.Name = "hexOutputToolStripMenuItem";
            this.hexOutputToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.hexOutputToolStripMenuItem.Text = "Hex output";
            this.hexOutputToolStripMenuItem.Click += new System.EventHandler(this.hexOutputToolStripMenuItem_Click);
            // 
            // decOutputToolStripMenuItem
            // 
            this.decOutputToolStripMenuItem.CheckOnClick = true;
            this.decOutputToolStripMenuItem.Name = "decOutputToolStripMenuItem";
            this.decOutputToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.decOutputToolStripMenuItem.Text = "Dec output";
            this.decOutputToolStripMenuItem.Click += new System.EventHandler(this.decOutputToolStripMenuItem_Click);
            // 
            // charOutputToolStripMenuItem
            // 
            this.charOutputToolStripMenuItem.CheckOnClick = true;
            this.charOutputToolStripMenuItem.Name = "charOutputToolStripMenuItem";
            this.charOutputToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.charOutputToolStripMenuItem.Text = "Char output";
            this.charOutputToolStripMenuItem.Click += new System.EventHandler(this.charOutputToolStripMenuItem_Click);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 85);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(643, 20);
            this.textBox1.TabIndex = 1;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(643, 105);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // contextMenuStrip_node
            // 
            this.contextMenuStrip_node.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem_open,
            this.removeToolStripMenuItem,
            this.toolStripMenuItem3,
            this.toolStripSeparator6,
            this.addNewToolStripMenuItem,
            this.toolStripSeparator5,
            this.updateToolStripMenuItem});
            this.contextMenuStrip_node.Name = "contextMenuStrip1";
            this.contextMenuStrip_node.Size = new System.Drawing.Size(122, 126);
            this.contextMenuStrip_node.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip_node_Closed);
            // 
            // openToolStripMenuItem_open
            // 
            this.openToolStripMenuItem_open.Name = "openToolStripMenuItem_open";
            this.openToolStripMenuItem_open.Size = new System.Drawing.Size(121, 22);
            this.openToolStripMenuItem_open.Text = "Open";
            this.openToolStripMenuItem_open.Click += new System.EventHandler(this.openToolStripMenuItem_open_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(121, 22);
            this.toolStripMenuItem3.Text = "Rename";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(118, 6);
            // 
            // addNewToolStripMenuItem
            // 
            this.addNewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sourceToolStripMenuItem,
            this.folderToolStripMenuItem1});
            this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
            this.addNewToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.addNewToolStripMenuItem.Text = "Add new";
            // 
            // sourceToolStripMenuItem
            // 
            this.sourceToolStripMenuItem.Name = "sourceToolStripMenuItem";
            this.sourceToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.sourceToolStripMenuItem.Text = "Source";
            this.sourceToolStripMenuItem.Click += new System.EventHandler(this.sourceToolStripMenuItem_Click);
            // 
            // folderToolStripMenuItem1
            // 
            this.folderToolStripMenuItem1.Name = "folderToolStripMenuItem1";
            this.folderToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.folderToolStripMenuItem1.Text = "Folder";
            this.folderToolStripMenuItem1.Click += new System.EventHandler(this.folderToolStripMenuItem1_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(118, 6);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.updateToolStripMenuItem.Text = "Update";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // splitContainer_main
            // 
            this.splitContainer_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_main.Location = new System.Drawing.Point(0, 24);
            this.splitContainer_main.Name = "splitContainer_main";
            // 
            // splitContainer_main.Panel1
            // 
            this.splitContainer_main.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer_main.Panel2
            // 
            this.splitContainer_main.Panel2.Controls.Add(this.splitContainer_editor);
            this.splitContainer_main.Size = new System.Drawing.Size(832, 453);
            this.splitContainer_main.SplitterDistance = 185;
            this.splitContainer_main.TabIndex = 17;
            this.splitContainer_main.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_main_SplitterMoved);
            // 
            // splitContainer_editor
            // 
            this.splitContainer_editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_editor.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_editor.Name = "splitContainer_editor";
            this.splitContainer_editor.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_editor.Panel1
            // 
            this.splitContainer_editor.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer_editor.Panel2
            // 
            this.splitContainer_editor.Panel2.Controls.Add(this.textBox1);
            this.splitContainer_editor.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer_editor.Size = new System.Drawing.Size(643, 453);
            this.splitContainer_editor.SplitterDistance = 344;
            this.splitContainer_editor.TabIndex = 0;
            this.splitContainer_editor.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_editor_SplitterMoved);
            // 
            // loadingCircle1
            // 
            this.loadingCircle1.Active = true;
            this.loadingCircle1.BackColor = System.Drawing.Color.Transparent;
            this.loadingCircle1.Color = System.Drawing.Color.DimGray;
            this.loadingCircle1.InnerCircleRadius = 5;
            this.loadingCircle1.Location = new System.Drawing.Point(115, 406);
            this.loadingCircle1.Name = "loadingCircle1";
            this.loadingCircle1.NumberSpoke = 12;
            this.loadingCircle1.OuterCircleRadius = 11;
            this.loadingCircle1.RotationSpeed = 45;
            this.loadingCircle1.Size = new System.Drawing.Size(90, 90);
            this.loadingCircle1.SpokeThickness = 2;
            this.loadingCircle1.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle1.TabIndex = 18;
            this.loadingCircle1.Text = "loadingCircle1";
            this.loadingCircle1.Visible = false;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 502);
            this.Controls.Add(this.loadingCircle1);
            this.Controls.Add(this.splitContainer_main);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Editor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Editor_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.Editor_LocationChanged);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip_node.ResumeLayout(false);
            this.splitContainer_main.Panel1.ResumeLayout(false);
            this.splitContainer_main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_main)).EndInit();
            this.splitContainer_main.ResumeLayout(false);
            this.splitContainer_editor.Panel1.ResumeLayout(false);
            this.splitContainer_editor.Panel2.ResumeLayout(false);
            this.splitContainer_editor.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_editor)).EndInit();
            this.splitContainer_editor.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog_source;
        private System.Windows.Forms.OpenFileDialog openFileDialog_binnary;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_source;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_binnary;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem runDebugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveCompiledToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCompiledToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem saveOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSourceFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addHeaderFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem precompileAllToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_node;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem_open;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compileOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem outputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hexOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem charOutputToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer_main;
        private System.Windows.Forms.SplitContainer splitContainer_editor;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private MRG.Controls.UI.LoadingCircle loadingCircle1;
    }
}

