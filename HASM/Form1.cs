using HASMLib;
using HASMLib.SyntaxTokens;
using HASMLib.SyntaxTokens.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private void button1_Click(object sender, EventArgs e)
        {
            HASMMachine machine = new HASMMachine(0xFFFFFF, 0xFFFFFFF, 0xFFFFFF);
            machine.SetRegisters("R{0}", 24);
            machine.ClearRegisters();

			HASMSource source = new HASMSource (richTextBox1.Text);

			ParseError pe;
			var parseResult = source.Parse (machine, out pe);

			if (pe != null) {
				MessageBox.Show (string.Format ("{0} at (Line: {1}: Index: {2})", pe.Type, pe.Line, pe.Index),
					"Error", MessageBoxButtons.OK);
			} else
				MessageBox.Show ("OK!");
        }
    }
}
