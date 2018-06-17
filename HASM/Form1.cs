using HASMLib;
using HASMLib.Core;
using HASMLib.Parser;
using HASMLib.Runtime;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HASM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<UInt12> Output;

        public static string ToPrettyFormat(TimeSpan span)
        {
            if (span == TimeSpan.Zero) return "0 minutes";

            var sb = new StringBuilder();
            if (span.Days > 0)
                sb.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : String.Empty);
            if (span.Hours > 0)
                sb.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : String.Empty);
            if (span.Minutes > 0)
                sb.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : String.Empty);
            if (span.Seconds > 0)
                sb.AppendFormat("{0} second{1} ", span.Seconds, span.Seconds > 1 ? "s" : String.Empty);
            if (span.Milliseconds > 0)
                sb.AppendFormat("{0} ms", span.Milliseconds);
            return sb.ToString();

        }

        public void OutputText(bool isText)
        {
            if (!isText)
            {
                richTextBox_output.Text = string.Join(", ", Output.Select(p => "0x" + ((int)p).ToString("X")));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HASMMachine machine = new HASMMachine(0xFFFFFF, 0xFFFFFFF, 0xFFFFFF);
            machine.SetRegisters("R{0}", 24);
            machine.ClearRegisters();

			HASMSource source = new HASMSource (machine, richTextBox_source.Text);
			ParseError pe = source.Parse();


			if (pe != null) 
			{
				MessageBox.Show (pe.ToString(), "Error", MessageBoxButtons.OK);
			}
			else
			{
                label_info.Text = string.Format("Parsed in: {0}. Size: {1} TBN(s)\n", ToPrettyFormat(source.ParseTime), source.UsedFlash);

                IOStream stream = new IOStream();
                RuntimeMachine runtimeMachine = machine.CreateRuntimeMachine(source, stream);
                runtimeMachine.Run();
                Output = stream.ReadAll();

                OutputText(false);
                label_output.Text = string.Format("Output ( {0} )", (Output.Count == 0 ? "empty" : Output.Count.ToString() + "TBN(s)"));
                label_info.Text += string.Format("Run in: {0}. Steps: {1}", ToPrettyFormat(runtimeMachine.TimeOfRunning), runtimeMachine.Ticks);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            OutputText(checkBox_outputType.Checked);
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            if(openFileDialog_source.ShowDialog() == DialogResult.OK)
            {
                richTextBox_source.Text = File.ReadAllText(openFileDialog_source.FileName);
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (saveFileDialog_source.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog_source.FileName, richTextBox_source.Text);
            }
        }
    }
}
