using FastColoredTextBoxNS;
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

        private Regex BinRegex = new Regex(@"\W0[bB][0-1]{1,100}(_[sdq]){0,1}");
        private Regex DecRegex = new Regex(@"\W\d{1,30}(_[sdq]){0,1}");
        private Regex HexRegex = new Regex(@"\W0[xX][0-9A-Fa-f]{1,15}(_[sdq]){0,1}");

        public void Close()
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
                        break;
                    case DialogResult.No:
                        collection.Remove(this);
                        Dispose(true);
                        break;
                    case DialogResult.Cancel:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                collection.Remove(this);
                Dispose(true);
            }
        }

        public TextEditor(string path)
        {
            TextBox = new FastColoredTextBox();
            TextBox.Dock = DockStyle.Fill;
            TextBox.Text = File.ReadAllText(path);

            DisplayName = path.Remove(0, path.Replace('\\', '/').LastIndexOf('/') + 1) + "  [x]";

            Text = DisplayName;

            TextBox.VisibleRangeChanged += (obj, args) =>
            {
                TextBox.VisibleRange.ClearStyle(StyleIndex.All);
                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.BrownStyle, LabelRegex);

                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.BlueStyle, BinRegex);
                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.BlueStyle, DecRegex);
                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.BlueStyle, HexRegex);

                TextBox.VisibleRange.SetStyle(TextBox.SyntaxHighlighter.GreenStyle, CommentRegex);
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
            Text = DisplayName;
            IsChanged = false;
            File.WriteAllText(Path, TextBox.Text);
        }

        public bool IsChanged = false;

        private string DisplayName;
        public string Path;

        public FastColoredTextBox TextBox { get; }
    }
}
