using HASMLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HASM
{
    public partial class CompileOptions : Form
    {
        public CompileOptions(CompileConfig cfg)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            config = cfg;
            textBox_ram.Text = cfg.RAM.ToString();
            textBox_flash.Text = cfg.Flash.ToString();
            textBox_eeprom.Text = cfg.EEPROM.ToString();
            textBox_registerCount.Text = cfg.RegisterCount.ToString();
            textBox_registerNameFormat.Text = cfg.RegisterNameFormat;

            checkedListBox_bf.Items.Clear();
            foreach(var a in Enum.GetNames(typeof(HASMMachineBannedFeatures)))
            {
                checkedListBox_bf.Items.Add(a, cfg.BannedFeatures.HasFlag((HASMMachineBannedFeatures)Enum.Parse(typeof(HASMMachineBannedFeatures), a)));
            }

            dataGridView1.Rows.Clear();
            foreach (var a in cfg.Defines)
            {
                if (string.IsNullOrEmpty(a.Name))
                    continue;

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell
                {
                    Value = a.Name
                };

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell
                {
                    Value = a.Value
                };

                row.Cells.Add(cell1);
                row.Cells.Add(cell2);

                dataGridView1.Rows.Add(row);
            }
        }

        public CompileConfig config;

        private void button_ok_Click(object sender, EventArgs e)
        {
            config.EEPROM = int.Parse(textBox_eeprom.Text);
            config.Flash = int.Parse(textBox_flash.Text);
            config.RAM = int.Parse(textBox_ram.Text);
            config.RegisterNameFormat = textBox_registerNameFormat.Text;
            config.RegisterCount = int.Parse(textBox_registerCount.Text);

            HASMMachineBannedFeatures bf = 0;
            foreach(var a in checkedListBox_bf.Items)
            {
                string name = (string)a;
                if (checkedListBox_bf.CheckedItems.Contains(a))
                    bf |= (HASMMachineBannedFeatures)Enum.Parse(typeof(HASMMachineBannedFeatures), name);
            }

            List<Define> defines = new List<Define>();
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                string name = (string)row.Cells[0].Value;
                string value = (string)row.Cells[1].Value;
                defines.Add(new Define(name, value));
            }

            config.BannedFeatures = bf;
            config.Defines = defines;

            CompileConfig.ToFile(config.FileName, config);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
