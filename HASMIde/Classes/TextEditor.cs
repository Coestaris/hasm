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

namespace HASM.Classes
{
    public partial class TextEditor : TabPage
    {
        private string ErrorString;
        private ParseError ParseError;
        private string Directory;
        private ParseTaskRunner TaskRunner;
        private AutocompleteMenu popupMenu;

        public bool IsChanged = false;
        public string DisplayName;
        public string Path;
        public FastColoredTextBox TextBox;
        public int HighlightedLine = -1;
        private ToolStripLabel toolStripLabel;
        private Editor Parrent;

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
            InitPlatformSpecificRegexes();

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

            List<AutocompleteItem> items = new List<AutocompleteItem>();
            popupMenu = new AutocompleteMenu(TextBox);
            popupMenu.ImageList = AutocompleteImageList;
            foreach (var a in Keywords)
                items.Add(new AutocompleteItem()
                {
                    Text = a,
                    ToolTipText = "Keyword " + a,
                    ToolTipTitle = "Keyword " + a,
                    ImageIndex = keyword
                });

            foreach (var a in HASMLib.Parser.SyntaxTokens.Expressions.Expression.Functions)
                items.Add(new AutocompleteItem()
                {
                    Text = a.FunctionString,
                    ToolTipText = "Keyword " + a,
                    ToolTipTitle = "Keyword " + a,
                    ImageIndex = function
                });


            InstructionRegexes = new List<Regex>();
            foreach (var a in HASMLib.Parser.SyntaxTokens.SourceLines.SourceLineInstruction.Instructions)
                items.Add(new AutocompleteItem()
                {
                    Text = a.NameString,
                    ToolTipText = "Keyword " + a,
                    ToolTipTitle = "Keyword " + a,
                    ImageIndex = instruction
                });


            PreprocessorRegexes = new List<Regex>();
            foreach (var a in HASMLib.Parser.SyntaxTokens.Preprocessor.PreprocessorDirective.PreprocessorDirectives)
                items.Add(new AutocompleteItem()
                {
                    Text = "#" + a.Name,
                    ToolTipText = "Keyword " + a,
                    ToolTipTitle = "Keyword " + a,
                    ImageIndex = preprocessorDir
                });


            popupMenu.Items.SetAutocompleteItems(items);
            popupMenu.MinFragmentLength = 1;
            popupMenu.Items.MaximumSize = new Size(200, 300);
            popupMenu.Items.Width = 200;

            HASMSource source = new HASMSource(Parrent.Machine, TextBox.Text);
            TaskRunner = new ParseTaskRunner(source);

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
            if (e.KeyData == (Keys.Space | Keys.Control))
            {
                //forced show (MinFragmentLength will be ignored)
                popupMenu.Show(true);
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                Save();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.W)
            {
                Close();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.F5)
            {
                Save();
                Editor.Self.Run(Path);
                e.Handled = true;
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

                            HighlightBaseSyntax();
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

            HighlightBaseSyntax();

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

        public void HighlightBaseSyntax()
        {
            TextBox.Range.SetStyle(CommentStyle, CommentRegex);
            TextBox.Range.SetStyle(DirectiveKeywordStyle, DirectiveKeywordRegex);
            TextBox.Range.SetStyle(KeywordStyle, KeywordRegex);
            TextBox.Range.SetStyle(BaseTypesNameStyle, BaseTypeRegex);


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
