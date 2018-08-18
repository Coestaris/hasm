using FastColoredTextBoxNS;
using HASMLib;
using HASMLib.Core.BaseTypes;
using HASMLib.Parser;
using HASMLib.Parser.SourceParsing;
using HASMLib.Runtime.Structures;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.TabControl;

namespace HASM
{
    public class TextEditor : TabPage
    {
        private static readonly List<string> Keywords = new List<string>()
        {
            Function.EntryPointKeyword,
            Function.ParameterKeyword,
            Function.ReturnKeyword,
            Function.SelfParameter,
            Function.StaticKeyword,
            Class.AbstractKeyword,
            Class.SealedKeyword,
            Field.TypeKeyword,

            Function.OverrideKeyword,
            Function.VirtualKeyword,

            AccessModifier.Default.ToString().ToLower(),
            AccessModifier.Inner.ToString().ToLower(),
            AccessModifier.Private.ToString().ToLower(),
            AccessModifier.Public.ToString().ToLower(),
        };

        private static readonly string KeywordRegexTemplate = @"(?<=[,\.\s:()])({0})(?=[,\.\s:()])";

        private static readonly Style ErrorStyle = new WavyLineStyle(255, Color.Red);

        private static readonly Style CommentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private static readonly Style DirectiveKeywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        private static readonly Style KeywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private static readonly Style RegularStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        private static readonly Style BaseTypesNameStyle = new TextStyle(Brushes.DarkGreen, null, FontStyle.Regular);
        private static readonly Style AssemblyNameStyle = new TextStyle(Brushes.DarkGreen, null, FontStyle.Regular);

        private static readonly Style FunctionNameStyle = new TextStyle(new SolidBrush(Color.FromArgb(167, 60, 193)), null, FontStyle.Regular);
        private static readonly Style FieldNameStyle = new TextStyle(new SolidBrush(Color.FromArgb(140, 19, 19)), null, FontStyle.Regular);
        private static readonly Style ClassNameStyle = new TextStyle(new SolidBrush(Color.FromArgb(50, 109, 102)), null, FontStyle.Regular);

        private static readonly Style StringStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
        private static readonly Style LabelStyle = new TextStyle(Brushes.Coral, null, FontStyle.Regular);
        private static readonly Style VariableStyle = new TextStyle(Brushes.DarkBlue, null, FontStyle.Regular);
        private static readonly Style BinNumberStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        private static readonly Style DecNumberStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        private static readonly Style HexNumberStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        private static readonly Style InstructionStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private static readonly Style PreprocessorStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        private static readonly Style BuiltinFunctionsStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);


        private static readonly Regex LabelRegex = new Regex(@"(?<=\W){1,100}:", RegexOptions.Multiline);
        private static readonly Regex CommentRegex = new Regex(@";.{0,}$", RegexOptions.Multiline);
        private static readonly Regex RegisterRegex = new Regex(@"(?<=\W)R\d{1,2}", RegexOptions.Multiline);
        private static readonly Regex DirectiveKeywordRegex = new Regex(@"(?<=\.)assembly|function|field|class|constructor(?=[\s(])");
        private static readonly Regex KeywordRegex = CreateRegex(Keywords);

        private static readonly Regex BinRegex = new Regex(@"(?<=\W)0[bB][0-1]{1,100}(_[sdq]){0,1}");
        private static readonly Regex DecRegex = new Regex(@"(?<=\W)\d{1,30}(_[sdq]){0,1}");
        private static readonly Regex HexRegex = new Regex(@"(?<=\W)0[xX][0-9A-Fa-f]{1,15}(_[sdq]){0,1}");
        private static readonly Regex String1Regex = new Regex("\\\".*\\\"");
        private static readonly Regex String2Regex = new Regex(@"<.*>");

        private static Regex BaseTypeRegex;
        private static List<Regex> FunctionRegexes;
        private static List<Regex> InstructionRegexes;
        private static List<Regex> PreprocessorRegexes;

        private string ErrorString;
        private ParseError ParseError;
        private string Directory;
        private ParseTaskRunner TaskRunner;

        public bool IsChanged = false;
        public string DisplayName;
        public string Path;
        public FastColoredTextBox TextBox;
        public int HighlightedLine = -1;
        private ToolStripLabel toolStripLabel;
        private Editor Parrent;

        private static void InitPlatformSpecificRegexes()
        {
            if (BaseTypeRegex != null) return;

            List<string> baseTypeNames = new List<string>();
            baseTypeNames.AddRange(BaseIntegerType.Types.Select(p => p.Name));
            baseTypeNames.AddRange(new List<string>()
            {
                "array",
                "string",
                "void"
            });


            BaseTypeRegex = CreateRegex(baseTypeNames);
            FunctionRegexes = new List<Regex>();
            foreach (var item in HASMLib.Parser.SyntaxTokens.Expressions.Expression.Functions)
                FunctionRegexes.Add(new Regex($"{item.FunctionString}"));

            InstructionRegexes = new List<Regex>();
            foreach (var item in HASMLib.Parser.SyntaxTokens.SourceLines.SourceLineInstruction.Instructions)
                InstructionRegexes.Add(new Regex($"\\s{item.NameString}\\s"));

            PreprocessorRegexes = new List<Regex>();
            foreach (var item in HASMLib.Parser.SyntaxTokens.Preprocessor.PreprocessorDirective.PreprocessorDirectives)
                PreprocessorRegexes.Add(new Regex($"#{item.Name}\\s"));

        }

        private static Regex CreateRegex(IEnumerable<string> collection)
        {
            return new Regex(string.Format(KeywordRegexTemplate, string.Join("|", collection)));
        }

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
        
        public TextEditor(string path, Control parent)
        {
            Parrent = parent as Editor;
            if (!File.Exists(path))
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

            HASMSource source = new HASMSource(Parrent.Machine, TextBox.Text);
            TaskRunner = new ParseTaskRunner(source);

            InitPlatformSpecificRegexes();

            Directory = new FileInfo(path).Directory.FullName;
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
                TextBox.SyntaxHighlighter.InitStyleSchema(Language.CSharp);
                TextBox.ToolTipDelay = 200;
                TextBox.ToolTip = new ToolTip();
                TextBox.DelayedTextChangedInterval = 200;
                TextBox.ToolTipNeeded += TextBox_ToolTipNeeded;
                TextBox.TextChangedDelayed += TextBox_TextChangedDelayed;
            }

            TextBox.TextChanged += TextBox_TextChanged;
            TextBox.KeyDown += TextBox_KeyDown;
            TextBox.HighlightingRangeType = HighlightingRangeType.ChangedRange;
            Path = path;

            Controls.Add(TextBox);
            toolStripLabel = Parrent.toolStripLabel1;
        }

        private void OutputText(string text)
        {
            if ((Parent as TabControl).SelectedTab == this)
                toolStripLabel.Text = text;
        }

        private void TextBox_ToolTipNeeded(object sender, ToolTipNeededEventArgs e)
        {
            if (ErrorString != null)
                if (ParseError.Line != -1 && ParseError.Line == e.Place.iLine) //TODO:!!!!
                {
                    e.ToolTipTitle = "Error";
                    e.ToolTipIcon = ToolTipIcon.Error;
                    if (ParseError.FileName.Contains("tmp"))
                    {
                        e.ToolTipText = ParseError.ToString(Path);
                    }
                    else
                    {
                        e.ToolTipText = ParseError.ToString();
                    }
                }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                Save();
            if (e.Control && e.KeyCode == Keys.W)
                Close();
            if (e.KeyCode == Keys.F5)
            {
                Save();
                Editor.Self.Run(Path);
            }
        }

        private void TextBox_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            TaskRunner.Source = new HASMSource(Parrent.Machine, TextBox.Text, Directory);
            TaskRunner.Run();
            if (TaskRunner.Status == ParseTaskStatus.Failed)
            {
                ParseError error = TaskRunner.Tasks[TaskRunner.FailedTaskIndex].Error;
                try
                {
                    if (error.FileName.Contains("tmp"))
                    {
                        //Current file
                        if (error.Line != -1)
                        {
                            TextBox.Range.ClearStyle(ErrorStyle);
                            TextBox.Range.SetStyle(ErrorStyle, Regex.Escape(TextBox[error.Line].Text.Trim()));
                            ErrorString = TextBox[error.Line].Text;
                        }
                        OutputText(error.ToString(Path));
                    }
                    else
                    {
                        OutputText(error.ToString());
                    }
                }
                catch
                {

                }
                ParseError = error;
                System.Console.WriteLine(error);
                return;
            }

            OutputText("");
            System.Console.WriteLine("Compiled!");

            ErrorString = null;
            ParseError = null;

            TextBox.Range.ClearStyle(StyleIndex.All);

            TextBox.Range.SetFoldingMarkers(
                HASMLib.Parser.SyntaxTokens.Structure.CodeBlock.BlockOpened,
                HASMLib.Parser.SyntaxTokens.Structure.CodeBlock.BlockClosed);

            TextBox.Range.SetStyle(CommentStyle, CommentRegex);
            TextBox.Range.SetStyle(DirectiveKeywordStyle, DirectiveKeywordRegex);
            TextBox.Range.SetStyle(KeywordStyle, KeywordRegex);
            TextBox.Range.SetStyle(BaseTypesNameStyle, BaseTypeRegex);


            if (TaskRunner.Source.Assembly != null)
            {
                Regex classNames = CreateRegex(TaskRunner.Source.Assembly.AllClasses.Select(p => p.Name));
                Regex functionNames = CreateRegex(TaskRunner.Source.Assembly.AllFunctions.Select(p => p.Name));
                Regex fieldNames = CreateRegex(TaskRunner.Source.Assembly.AllFields.Select(p => p.Name));

                TextBox.Range.SetStyle(AssemblyNameStyle, TaskRunner.Source.Assembly.Name);
                TextBox.Range.SetStyle(ClassNameStyle, classNames);
                TextBox.Range.SetStyle(FunctionNameStyle, functionNames);
                TextBox.Range.SetStyle(FieldNameStyle, fieldNames);
            }

            TextBox.Range.SetStyle(StringStyle, String1Regex);
            TextBox.Range.SetStyle(StringStyle, String2Regex);
            TextBox.Range.SetStyle(LabelStyle, LabelRegex);
            TextBox.Range.SetStyle(VariableStyle, RegisterRegex);
            TextBox.Range.SetStyle(DecNumberStyle, BinRegex);
            TextBox.Range.SetStyle(DecNumberStyle, DecRegex);
            TextBox.Range.SetStyle(DecNumberStyle, HexRegex);

            foreach (var item in InstructionRegexes)
                TextBox.Range.SetStyle(InstructionStyle, item);

            foreach (var item in PreprocessorRegexes)
                TextBox.Range.SetStyle(PreprocessorStyle, item);

            foreach (var item in FunctionRegexes)
                TextBox.Range.SetStyle(BuiltinFunctionsStyle, item);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsChanged = true;
            Text = DisplayName + "*";
            if (HighlightedLine != -1)
            {
                TextBox[HighlightedLine].BackgroundBrush = Brushes.Transparent;
                HighlightedLine = -1;
            }
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
    }
}
