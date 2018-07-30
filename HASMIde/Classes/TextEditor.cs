using FastColoredTextBoxNS;
using HASMLib.Parser.SyntaxTokens;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.TabControl;

namespace HASM
{
    public class TextEditor : TabPage
    {
        private static Style CommentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private static Style KeywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private static Style RegularStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);

        private static Style AssemblyNameStyle = new TextStyle(Brushes.DarkGreen, null, FontStyle.Regular);
        private static Style UnitsNameStyle = new TextStyle(Brushes.DarkViolet, null, FontStyle.Regular);
        private static Style ClassNameStyle = new TextStyle(Brushes.DarkSlateBlue, null, FontStyle.Regular);

        private static Style StringStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
        private static Style LabelStyle = new TextStyle(Brushes.Coral, null, FontStyle.Regular);
        private static Style VariableStyle = new TextStyle(Brushes.DarkBlue, null, FontStyle.Regular);
        private static Style BinNumberStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        private static Style DecNumberStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        private static Style HexNumberStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);

        private static Style InstructionStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private static Style PreprocessorStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        private static Style BuiltinFunctionsStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);


        private static Regex LabelRegex = new Regex(@"(?<=\W){1,100}:", RegexOptions.Multiline);
        private static Regex CommentRegex = new Regex(@";.{0,}$", RegexOptions.Multiline);
        private static Regex RegisterRegex = new Regex(@"(?<=\W)R\d{1,2}", RegexOptions.Multiline);

        private static Regex AssemblyRegex = new Regex(@"(?<=\.assembly\s)\w*");
        private static Regex FullUnitsRegex = new Regex(@".(function|field|constructor)\(.*\)\s+\w*");
        private static Regex PartialUnitsRegex = new Regex(@".(function|field|constructor)\([^()]*?\)");

        private static Regex FullClassRegex = new Regex(@".class\(.*\)\s+\w*");
        private static Regex PartialClassRegex = new Regex(@".class\([^()]*?\)");


        private static List<Regex> KeywordRegexes = new List<Regex>()
        {
            new Regex(@"(?<=\.)assembly"),
            new Regex(@"(?<=\.)class"),
            new Regex(@"(?<=\.)constructor"),
            new Regex(@"(?<=\.)function"),
            new Regex(@"(?<=\.)field"),
        };

        private static Regex BinRegex = new Regex(@"(?<=\W)0[bB][0-1]{1,100}(_[sdq]){0,1}");
        private static Regex DecRegex = new Regex(@"(?<=\W)\d{1,30}(_[sdq]){0,1}");
        private static Regex HexRegex = new Regex(@"(?<=\W)0[xX][0-9A-Fa-f]{1,15}(_[sdq]){0,1}");

        private static Regex String1Regex = new Regex("\\\".*\\\"");
        private static Regex String2Regex = new Regex(@"<.*>");

        public int HighlightedLine = -1;

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

            TextBox = new FastColoredTextBox
            {
                Dock = DockStyle.Fill,
                Text = File.ReadAllText(path),

                BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy1,
                AutoCompleteBrackets = true,
                LeftBracket = '(',
                RightBracket = ')',
                
            };

            DisplayName = path.Remove(0, path.Replace('\\', '/').LastIndexOf('/') + 1) + "  [x]";

            Text = DisplayName;

            if (DisplayName.Contains(".cfg"))
            {
                TextBox.VisibleRangeChanged += (obj, args) =>
                {
                    TextBox.VisibleRange.ClearStyle(StyleIndex.All);
                    TextBox.SyntaxHighlighter.XMLSyntaxHighlight(TextBox.VisibleRange);
                    TextBox.SyntaxHighlighter.XmlTagNameStyle = TextBox.SyntaxHighlighter.BlueStyle;
                };
            }
            else
            {
                List<Regex> FunctionRegexes = new List<Regex>();
                foreach (var item in HASMLib.Parser.SyntaxTokens.Expressions.Expression.Functions)
                    FunctionRegexes.Add(new Regex($"{item.FunctionString}"));

                List<Regex> InstructionRegexes = new List<Regex>();
                foreach (var item in HASMLib.Parser.SyntaxTokens.SourceLines.SourceLineInstruction.Instructions)
                    InstructionRegexes.Add(new Regex($"\\s{item.NameString}\\s"));

                List<Regex> PreprocessorRegexes = new List<Regex>();
                foreach (var item in HASMLib.Parser.SyntaxTokens.Preprocessor.PreprocessorDirective.PreprocessorDirectives)
                    PreprocessorRegexes.Add(new Regex($"#{item.Name}\\s"));

                TextBox.TextChanged += (obj, args) =>
                {
                    if(HighlightedLine != -1)
                  
                    {
                        TextBox[HighlightedLine].BackgroundBrush = Brushes.Transparent;
                        HighlightedLine = -1;
                    }
                };

                TextBox.SyntaxHighlighter.InitStyleSchema(Language.CSharp);

                TextBox.VisibleRangeChanged += (obj, args) =>
                {
                    TextBox.VisibleRange.ClearStyle(StyleIndex.All);

                    TextBox.VisibleRange.SetFoldingMarkers(
                        HASMLib.Parser.SyntaxTokens.Structure.CodeBlock.BlockOpened,
                        HASMLib.Parser.SyntaxTokens.Structure.CodeBlock.BlockClosed);

                    TextBox.VisibleRange.SetStyle(CommentStyle, CommentRegex);

                    foreach (var item in KeywordRegexes)
                        TextBox.VisibleRange.SetStyle(KeywordStyle, item);

                    TextBox.VisibleRange.SetStyle(RegularStyle, PartialClassRegex);
                    TextBox.VisibleRange.SetStyle(RegularStyle, PartialUnitsRegex);

                    TextBox.VisibleRange.SetStyle(AssemblyNameStyle, AssemblyRegex);
                    TextBox.VisibleRange.SetStyle(ClassNameStyle, FullClassRegex);
                    TextBox.VisibleRange.SetStyle(UnitsNameStyle, FullUnitsRegex);

                    TextBox.VisibleRange.SetStyle(StringStyle, String1Regex);
                    TextBox.VisibleRange.SetStyle(StringStyle, String2Regex);
                    
                    TextBox.VisibleRange.SetStyle(LabelStyle, LabelRegex);
                    TextBox.VisibleRange.SetStyle(VariableStyle, RegisterRegex);
                    TextBox.VisibleRange.SetStyle(BinNumberStyle, BinRegex);
                    TextBox.VisibleRange.SetStyle(DecNumberStyle, DecRegex);
                    TextBox.VisibleRange.SetStyle(HexNumberStyle, HexRegex);

                    foreach (var item in InstructionRegexes)
                        TextBox.VisibleRange.SetStyle(InstructionStyle, item);

                    foreach (var item in PreprocessorRegexes)
                        TextBox.VisibleRange.SetStyle(PreprocessorStyle, item);

                    foreach (var item in FunctionRegexes)
                        TextBox.VisibleRange.SetStyle(BuiltinFunctionsStyle, item);
                };
            }

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
                if (args.KeyCode == Keys.F5)
                {
                    Save();
                    Editor.Self.Run(Path);
                }
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

        public string DisplayName;
        public string Path;

        public FastColoredTextBox TextBox { get; }
    }
}
