namespace HASM
{
    partial class CompileOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.checkedListBox_bf = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_ram = new System.Windows.Forms.TextBox();
            this.textBox_flash = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_eeprom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_registerNameFormat = new System.Windows.Forms.TextBox();
            this.textBox_registerCount = new System.Windows.Forms.TextBox();
            this.label_base = new System.Windows.Forms.Label();
            this.comboBox_base = new System.Windows.Forms.ComboBox();
            this.richTextBox_incPath = new System.Windows.Forms.RichTextBox();
            this.label_incPath = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(353, 382);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 23);
            this.button_ok.TabIndex = 0;
            this.button_ok.Text = "Ok";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(272, 382);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 1;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // checkedListBox_bf
            // 
            this.checkedListBox_bf.FormattingEnabled = true;
            this.checkedListBox_bf.Location = new System.Drawing.Point(113, 103);
            this.checkedListBox_bf.Name = "checkedListBox_bf";
            this.checkedListBox_bf.Size = new System.Drawing.Size(315, 64);
            this.checkedListBox_bf.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "RAM";
            // 
            // textBox_ram
            // 
            this.textBox_ram.Location = new System.Drawing.Point(113, 15);
            this.textBox_ram.Name = "textBox_ram";
            this.textBox_ram.Size = new System.Drawing.Size(126, 20);
            this.textBox_ram.TabIndex = 4;
            // 
            // textBox_flash
            // 
            this.textBox_flash.Location = new System.Drawing.Point(113, 41);
            this.textBox_flash.Name = "textBox_flash";
            this.textBox_flash.Size = new System.Drawing.Size(126, 20);
            this.textBox_flash.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Flash";
            // 
            // textBox_eeprom
            // 
            this.textBox_eeprom.Location = new System.Drawing.Point(113, 67);
            this.textBox_eeprom.Name = "textBox_eeprom";
            this.textBox_eeprom.Size = new System.Drawing.Size(126, 20);
            this.textBox_eeprom.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "EEPROM";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Banned Features";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 265);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Defines";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Name,
            this.Value});
            this.dataGridView1.Location = new System.Drawing.Point(113, 265);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(315, 111);
            this.dataGridView1.TabIndex = 12;
            // 
            // Name
            // 
            this.Name.HeaderText = "Name";
            this.Name.Name = "Name";
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(245, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 26);
            this.label6.TabIndex = 13;
            this.label6.Text = "Register\r\nname format";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(245, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Register count";
            // 
            // textBox_registerNameFormat
            // 
            this.textBox_registerNameFormat.Location = new System.Drawing.Point(327, 15);
            this.textBox_registerNameFormat.Name = "textBox_registerNameFormat";
            this.textBox_registerNameFormat.Size = new System.Drawing.Size(101, 20);
            this.textBox_registerNameFormat.TabIndex = 15;
            // 
            // textBox_registerCount
            // 
            this.textBox_registerCount.Location = new System.Drawing.Point(327, 41);
            this.textBox_registerCount.Name = "textBox_registerCount";
            this.textBox_registerCount.Size = new System.Drawing.Size(101, 20);
            this.textBox_registerCount.TabIndex = 16;
            // 
            // label_base
            // 
            this.label_base.AutoSize = true;
            this.label_base.Location = new System.Drawing.Point(245, 71);
            this.label_base.Name = "label_base";
            this.label_base.Size = new System.Drawing.Size(56, 13);
            this.label_base.TabIndex = 17;
            this.label_base.Text = "ASM base";
            // 
            // comboBox_base
            // 
            this.comboBox_base.FormattingEnabled = true;
            this.comboBox_base.Items.AddRange(new object[] {
            "8",
            "12",
            "16"});
            this.comboBox_base.Location = new System.Drawing.Point(327, 68);
            this.comboBox_base.Name = "comboBox_base";
            this.comboBox_base.Size = new System.Drawing.Size(101, 21);
            this.comboBox_base.TabIndex = 18;
            this.comboBox_base.Text = "8";
            // 
            // richTextBox_incPath
            // 
            this.richTextBox_incPath.Location = new System.Drawing.Point(113, 173);
            this.richTextBox_incPath.Name = "richTextBox_incPath";
            this.richTextBox_incPath.Size = new System.Drawing.Size(315, 86);
            this.richTextBox_incPath.TabIndex = 19;
            this.richTextBox_incPath.Text = "";
            // 
            // label_incPath
            // 
            this.label_incPath.AutoSize = true;
            this.label_incPath.Location = new System.Drawing.Point(20, 176);
            this.label_incPath.Name = "label_incPath";
            this.label_incPath.Size = new System.Drawing.Size(71, 13);
            this.label_incPath.TabIndex = 20;
            this.label_incPath.Text = "Include paths";
            // 
            // CompileOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 417);
            this.Controls.Add(this.label_incPath);
            this.Controls.Add(this.richTextBox_incPath);
            this.Controls.Add(this.comboBox_base);
            this.Controls.Add(this.label_base);
            this.Controls.Add(this.textBox_registerCount);
            this.Controls.Add(this.textBox_registerNameFormat);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_eeprom);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_flash);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_ram);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkedListBox_bf);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_ok);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CompileOptions";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.CheckedListBox checkedListBox_bf;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_ram;
        private System.Windows.Forms.TextBox textBox_flash;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_eeprom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_registerNameFormat;
        private System.Windows.Forms.TextBox textBox_registerCount;
        private System.Windows.Forms.Label label_base;
        private System.Windows.Forms.ComboBox comboBox_base;
        private System.Windows.Forms.RichTextBox richTextBox_incPath;
        private System.Windows.Forms.Label label_incPath;
    }
}