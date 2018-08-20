using FastColoredTextBoxNS;
using HASMLib.Core.BaseTypes;
using HASMLib.Runtime.Structures;
using HASMLib.Runtime.Structures.Units;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HASM.Classes
{
    public partial class TextEditor : TabPage
    {
        private static readonly List<string> Keywords = new List<string>()
        {
            Function.EntryPointKeyword,
            Function.ParameterKeyword,
            Function.ReturnKeyword,
            Function.SelfParameter,
            BaseStructure.StaticKeyword,
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

        private const int class_public = 0;
        private const int class_protected = 1;
        private const int class_private = 2;
        private const int field_public = 3;
        private const int field_protected = 4;
        private const int field_private = 5;
        private const int method_public = 6;
        private const int method_protected = 7;
        private const int method_private = 8;
        private const int keyword = 9;
        private const int define = 10;
        private const int type = 11;
        private const int function = 12;
        private const int instruction = 13;
        private const int preprocessorDir = 14;

        private static readonly Dictionary<int, Bitmap> AutocompleteImages = new Dictionary<int, Bitmap>()
        {
			{ class_public,     new Bitmap($"Icons{PlatformSpecific.NameSeparator}Class_public.bmp")            },
			{ class_protected,  new Bitmap($"Icons{PlatformSpecific.NameSeparator}Class_protected.bmp")         },
			{ class_private,    new Bitmap($"Icons{PlatformSpecific.NameSeparator}Class_private.bmp")           },
			{ field_public,     new Bitmap($"Icons{PlatformSpecific.NameSeparator}Field_public.bmp")            },
			{ field_protected,  new Bitmap($"Icons{PlatformSpecific.NameSeparator}Field_protected.bmp")         },
			{ field_private,    new Bitmap($"Icons{PlatformSpecific.NameSeparator}Field_private.bmp")           },
			{ method_public,    new Bitmap($"Icons{PlatformSpecific.NameSeparator}Method_public.bmp")           },
			{ method_protected, new Bitmap($"Icons{PlatformSpecific.NameSeparator}Method_protected.bmp")        },
			{ method_private,   new Bitmap($"Icons{PlatformSpecific.NameSeparator}Method_private.bmp")          },
			{ keyword,          new Bitmap($"Icons{PlatformSpecific.NameSeparator}Keyword.bmp")                 },
			{ define,           new Bitmap($"Icons{PlatformSpecific.NameSeparator}Define.bmp")                  },
			{ type,             new Bitmap($"Icons{PlatformSpecific.NameSeparator}Type.bmp")                    },
			{ function,         new Bitmap($"Icons{PlatformSpecific.NameSeparator}BuiltinFunction.bmp")         },
			{ instruction,      new Bitmap($"Icons{PlatformSpecific.NameSeparator}Instruction.bmp")             },
			{ preprocessorDir,  new Bitmap($"Icons{PlatformSpecific.NameSeparator}PreprocessorDirective.bmp")   },
        };

        private static ImageList AutocompleteImageList;
        private static List<AutocompleteItem> AutoCompleteStaticValues = new List<AutocompleteItem>();

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
        private static readonly Regex String1Regex = new Regex("\\\".*?\\\"");
        private static readonly Regex String2Regex = new Regex(@"<.*>");

        private static Regex BaseTypeRegex;
        private static List<Regex> FunctionRegexes;
        private static List<Regex> InstructionRegexes;
        private static List<Regex> PreprocessorRegexes;

        private static void InitPlatformSpecificRegexes()
        {
            if (BaseTypeRegex != null) return;

            AutocompleteImageList = new ImageList();
            foreach (var a in AutocompleteImages)
                AutocompleteImageList.Images.Add(a.Value, Color.White);

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

            foreach (var a in Keywords)
                AutoCompleteStaticValues.Add(new AutocompleteItem()
                {
                    Text = a,
                    ToolTipTitle = "Keyword " + a,
                    ToolTipText = "Keyword " + a,
                    ImageIndex = keyword
                });

            foreach (var a in new List<string>{ "function", "assembly", "field", "class", "constructor" })
                AutoCompleteStaticValues.Add(new AutocompleteItem()
                {
                    Text = a,
                    ToolTipTitle = "Keyword " + a,
                    ToolTipText = "Keyword " + a,
                    ImageIndex = keyword
                });

            foreach (var a in HASMLib.Parser.SyntaxTokens.SourceLines.SourceLineInstruction.Instructions)
            {
                int counter = 1;
                AutoCompleteStaticValues.Add(new AutocompleteItem()
                {
                    Text = a.NameString,
                    ToolTipTitle = "Instruction " + a.NameString,
                    ToolTipText = $"Parameter count: {a.ParameterCount}\nParameter types:\n{string.Join("\n", a.ParameterTypes.Select(p => $"{counter++}. {p}"))}",
                    ImageIndex = instruction
                });
            }

            foreach (var a in HASMLib.Parser.SyntaxTokens.Preprocessor.PreprocessorDirective.PreprocessorDirectives)
                AutoCompleteStaticValues.Add(new AutocompleteItem()
                {
                    Text = "#" + a.Name,
                    ToolTipTitle = "Directriove " + a.Name,
                    ToolTipText = "Can add values " + a.CanAddNewLines,
                    ImageIndex = preprocessorDir
                });

            foreach(var a in BaseIntegerType.Types)
                AutoCompleteStaticValues.Add(new AutocompleteItem()
                {
                    Text = a.Name,
                    ToolTipTitle = "Base int type " + a.Name,
                    ToolTipText = $"A {a.Base}-bit {(a.IsSigned ? "signed" : "unsigned")} type\nMin value: {a.MinValue}\nMax value: {a.MaxValue}",
                    ImageIndex = type
                });

            foreach(var a in HASMLib.Parser.SyntaxTokens.Expressions.Expression.Functions)
                AutoCompleteStaticValues.Add(new AutocompleteItem()
                {
                    Text = a.FunctionString,
                    ToolTipTitle = "Function " + a.FunctionString,
                    ToolTipText = $"Takes {a.FunctionSteps} steps",
                    ImageIndex = function
                });
            AutoCompleteStaticValues = AutoCompleteStaticValues.OrderBy(p => p.Text).ToList();
        }

        private static Regex CreateRegex(IEnumerable<string> collection)
        {
            return new Regex(string.Format(KeywordRegexTemplate, string.Join("|", collection)));
        }
    }
}
