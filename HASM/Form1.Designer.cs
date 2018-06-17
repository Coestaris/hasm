namespace HASM
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_compile = new System.Windows.Forms.Button();
            this.richTextBox_source = new System.Windows.Forms.RichTextBox();
            this.richTextBox_output = new System.Windows.Forms.RichTextBox();
            this.label_source = new System.Windows.Forms.Label();
            this.label_output = new System.Windows.Forms.Label();
            this.label_info = new System.Windows.Forms.Label();
            this.checkBox_outputType = new System.Windows.Forms.CheckBox();
            this.button_loadCompiled = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.button_load = new System.Windows.Forms.Button();
            this.button_saveCompiled = new System.Windows.Forms.Button();
            this.openFileDialog_source = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog_binnary = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog_source = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog_binnary = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // button_compile
            // 
            this.button_compile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_compile.Location = new System.Drawing.Point(426, 383);
            this.button_compile.Name = "button_compile";
            this.button_compile.Size = new System.Drawing.Size(114, 23);
            this.button_compile.TabIndex = 0;
            this.button_compile.TabStop = false;
            this.button_compile.Text = "Compile\'n RUN";
            this.button_compile.UseVisualStyleBackColor = true;
            this.button_compile.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox_source
            // 
            this.richTextBox_source.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox_source.ImeMode = System.Windows.Forms.ImeMode.On;
            this.richTextBox_source.Location = new System.Drawing.Point(12, 41);
            this.richTextBox_source.Name = "richTextBox_source";
            this.richTextBox_source.ShowSelectionMargin = true;
            this.richTextBox_source.Size = new System.Drawing.Size(528, 213);
            this.richTextBox_source.TabIndex = 1;
            this.richTextBox_source.TabStop = false;
            this.richTextBox_source.Text = resources.GetString("richTextBox_source.Text");
            // 
            // richTextBox_output
            // 
            this.richTextBox_output.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox_output.Location = new System.Drawing.Point(12, 283);
            this.richTextBox_output.Name = "richTextBox_output";
            this.richTextBox_output.ReadOnly = true;
            this.richTextBox_output.Size = new System.Drawing.Size(528, 94);
            this.richTextBox_output.TabIndex = 2;
            this.richTextBox_output.TabStop = false;
            this.richTextBox_output.Text = "";
            // 
            // label_source
            // 
            this.label_source.AutoSize = true;
            this.label_source.Location = new System.Drawing.Point(12, 12);
            this.label_source.Name = "label_source";
            this.label_source.Size = new System.Drawing.Size(41, 13);
            this.label_source.TabIndex = 3;
            this.label_source.Text = "Source";
            // 
            // label_output
            // 
            this.label_output.AutoSize = true;
            this.label_output.Location = new System.Drawing.Point(12, 267);
            this.label_output.Name = "label_output";
            this.label_output.Size = new System.Drawing.Size(39, 13);
            this.label_output.TabIndex = 4;
            this.label_output.Text = "Output";
            // 
            // label_info
            // 
            this.label_info.AutoSize = true;
            this.label_info.Location = new System.Drawing.Point(12, 388);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(35, 13);
            this.label_info.TabIndex = 5;
            this.label_info.Text = "label3";
            // 
            // checkBox_outputType
            // 
            this.checkBox_outputType.AutoSize = true;
            this.checkBox_outputType.Location = new System.Drawing.Point(448, 260);
            this.checkBox_outputType.Name = "checkBox_outputType";
            this.checkBox_outputType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox_outputType.Size = new System.Drawing.Size(92, 17);
            this.checkBox_outputType.TabIndex = 6;
            this.checkBox_outputType.TabStop = false;
            this.checkBox_outputType.Text = "Output as text";
            this.checkBox_outputType.UseVisualStyleBackColor = true;
            this.checkBox_outputType.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button_loadCompiled
            // 
            this.button_loadCompiled.Location = new System.Drawing.Point(186, 258);
            this.button_loadCompiled.Name = "button_loadCompiled";
            this.button_loadCompiled.Size = new System.Drawing.Size(136, 23);
            this.button_loadCompiled.TabIndex = 7;
            this.button_loadCompiled.TabStop = false;
            this.button_loadCompiled.Text = "Load compiled and RUN";
            this.button_loadCompiled.UseVisualStyleBackColor = true;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(465, 12);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 8;
            this.button_save.TabStop = false;
            this.button_save.Text = "Save source";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_load
            // 
            this.button_load.Location = new System.Drawing.Point(384, 12);
            this.button_load.Name = "button_load";
            this.button_load.Size = new System.Drawing.Size(75, 23);
            this.button_load.TabIndex = 9;
            this.button_load.TabStop = false;
            this.button_load.Text = "Load source";
            this.button_load.UseVisualStyleBackColor = true;
            this.button_load.Click += new System.EventHandler(this.button_load_Click);
            // 
            // button_saveCompiled
            // 
            this.button_saveCompiled.Location = new System.Drawing.Point(328, 258);
            this.button_saveCompiled.Name = "button_saveCompiled";
            this.button_saveCompiled.Size = new System.Drawing.Size(114, 23);
            this.button_saveCompiled.TabIndex = 10;
            this.button_saveCompiled.TabStop = false;
            this.button_saveCompiled.Text = "Save compiled";
            this.button_saveCompiled.UseVisualStyleBackColor = true;
            // 
            // openFileDialog_source
            // 
            this.openFileDialog_source.FileName = "openFileDialog_source";
            // 
            // openFileDialog_binnary
            // 
            this.openFileDialog_binnary.FileName = "openFileDialog_binnary";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 413);
            this.Controls.Add(this.button_saveCompiled);
            this.Controls.Add(this.button_load);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_loadCompiled);
            this.Controls.Add(this.checkBox_outputType);
            this.Controls.Add(this.label_info);
            this.Controls.Add(this.label_output);
            this.Controls.Add(this.label_source);
            this.Controls.Add(this.richTextBox_output);
            this.Controls.Add(this.richTextBox_source);
            this.Controls.Add(this.button_compile);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_compile;
        private System.Windows.Forms.RichTextBox richTextBox_source;
        private System.Windows.Forms.RichTextBox richTextBox_output;
        private System.Windows.Forms.Label label_source;
        private System.Windows.Forms.Label label_output;
        private System.Windows.Forms.Label label_info;
        private System.Windows.Forms.CheckBox checkBox_outputType;
        private System.Windows.Forms.Button button_loadCompiled;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_load;
        private System.Windows.Forms.Button button_saveCompiled;
        private System.Windows.Forms.OpenFileDialog openFileDialog_source;
        private System.Windows.Forms.OpenFileDialog openFileDialog_binnary;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_source;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_binnary;
    }
}

