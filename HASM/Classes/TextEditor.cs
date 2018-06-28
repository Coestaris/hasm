﻿using FastColoredTextBoxNS;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.TabControl;

namespace HASM
{
    public class TextEditor : TabPage
    {
        private Regex LabelRegex = new Regex(@"^\w{1,100}:", RegexOptions.Multiline);
        private Regex CommentRegex = new Regex(@";.{0,}$", RegexOptions.Multiline);
        private Regex RegisterRegex = new Regex(@"R\d{0,2}", RegexOptions.Multiline);


        private Regex BinRegex = new Regex(@"\W0[bB][0-1]{1,100}(_[sdq]){0,1}");
        private Regex DecRegex = new Regex(@"\W\d{1,30}(_[sdq]){0,1}");
        private Regex HexRegex = new Regex(@"\W0[xX][0-9A-Fa-f]{1,15}(_[sdq]){0,1}");

        public bool Close()
        {
            TabPageCollection collection = (Parent as TabControl).TabPages;
            if (IsChanged)
            {
                switch (MessageBox.Show("Save changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        Save();
                        collection.Remove(this);
                        Dispose(true);
                        return true;

                    case DialogResult.No:
                        collection.Remove(this);
                        Dispose(true);
                        return true;

                    case DialogResult.Cancel:
                        return false;

                    default:
                        return false;
                }
            }
            else
            {
                collection.Remove(this);
                Dispose(true);
                return true;
            }
        }

        public TextEditor(string path)
        {
            if(!File.Exists(path))
            {
                MessageBox.Show($"Unable to find file {path}");
                return;
            }

            TextBox = new FastColoredTextBox();
            TextBox.Dock = DockStyle.Fill;
            TextBox.Text = File.ReadAllText(path);

            DisplayName = path.Remove(0, path.Replace('\\', '/').LastIndexOf('/') + 1) + "  [x]";

            Text = DisplayName;

            TextBox.VisibleRangeChanged += (obj, args) =>
            {
                TextBox.VisibleRange.ClearStyle(StyleIndex.All);

                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.GreenStyle, CommentRegex);


                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.BlackStyle, LabelRegex);

                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.MaroonStyle, RegisterRegex);
                
                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.GrayStyle, BinRegex);
                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.GrayStyle, DecRegex);
                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.GrayStyle, HexRegex);

                foreach (var item in HASMLib.Parser.SyntaxTokens.SourceLines.SourceLineInstruction.Instructions)
                {
                    TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.BlueStyle, item.NameString);
                }

            };

            TextBox.TextChanged += (obj, args) =>
            {
                IsChanged = true;
                Text = DisplayName + "*";
            };

            TextBox.KeyDown += (obj, args) =>
            {
                if (args.Control && args.KeyCode == Keys.S)
                    Save();
                if (args.Control && args.KeyCode == Keys.W)
                    Close();
            };

            TextBox.HighlightingRangeType = HighlightingRangeType.ChangedRange;
            Path = path;

            Controls.Add(TextBox);

        }

        public void Save()
        {
            if (IsChanged)
            {
                if(!File.Exists(Path))
                {
                    MessageBox.Show("File has been deleted or moved");
                    return;
                }

                Text = DisplayName;
                IsChanged = false;
                File.WriteAllText(Path, TextBox.Text);
            }
        }

        public bool IsChanged = false;

        private string DisplayName;
        public string Path;

        public FastColoredTextBox TextBox { get; }
    }
}
