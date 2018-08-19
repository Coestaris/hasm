using HASMLib.Parser.SyntaxTokens;
using HASMLib.Parser.SyntaxTokens.SourceLines;
using HASMLib.Parser.SyntaxTokens.Structure;
using System.Collections.Generic;

namespace HASMLib.Parser.SourceParsing.ParseTasks
{
    class StructureTask : ParseTask
    {
        public override string Name => "Building assembly structure";

        protected override void InnerReset() { }

        public CodeBlock GetLines(int startIndex, int endIndex, List<SourceLine> lines, out ParseError error, SourceLineDirective parent)
        {
            CodeBlock block = new CodeBlock(parent);
            SourceLineDirective sourceLineDirective = null;
            bool isSeekingEnd = false;
            bool startRequired = false;
            int openedIndex = 0;
            int seekLevel = 0;
            int currentLevel = 0;

            for (int i = startIndex; i < endIndex; i++)
            {
                if (SourceLineDirective.IsDirective(lines[i].Input))
                {
                    if (isSeekingEnd)
                        continue;

                    if(startRequired)
                    {
                        error = new ParseError(ParseErrorType.Directives_StartOfCodeBlockExpected,
                            lines[i].LineIndex, lines[i].FileName);
                        return null;
                    }

                    var dir = new SourceLineDirective(lines[i]);
                    error = dir.Parse();
                    if (error != null) return null;
                    lines[i] = dir.Cast(out error);
                    if (error != null) return null;
                    var directive = (lines[i] as SourceLineDirective);

                    if (directive.RequireCodeBlock)
                    {
                        directive.CodeBlock = block;
                        sourceLineDirective = directive;
                        startRequired = true;
                    }
                    else
                    {
                        block.ChildBlocks.Add(new CodeBlock(directive));
                    }
                }
                else if (SourceLineCodeBlockLimiter.IsCodeBlockLimiter(lines[i].Input))
                {
                    var lim = new SourceLineCodeBlockLimiter(lines[i]);
                    error = lim.Parse();
                    if (error != null) return null;

                    lines[i] = lim;

                    if(lim.IsOpening)
                    {
                        if (!isSeekingEnd)
                        {
                            if (!startRequired)
                            {
                                error = new ParseError(ParseErrorType.Directives_StartOfCodeBlockIsUnexpected,
                                    lines[i].LineIndex, lines[i].FileName);
                                return null;
                            }

                            seekLevel = currentLevel;
                            openedIndex = i;
                            isSeekingEnd = true;
                            startRequired = false;
                        }

                        currentLevel++;
                    }
                    else
                    {
                        if (startRequired)
                        {
                            error = new ParseError(ParseErrorType.Directives_StartOfCodeBlockExpected,
                                lines[i].LineIndex, lines[i].FileName);
                            return null;
                        }

                        if (isSeekingEnd && currentLevel - 1 == seekLevel)
                        {
                            sourceLineDirective.CodeBlock.ChildBlocks.Add(GetLines(openedIndex + 1, i,
                                lines, out error, sourceLineDirective));

                            if (error != null)
                                return null;

                            isSeekingEnd = false;
                            startRequired = false;
                        }
                        
                        if(currentLevel ==  0)
                        {
                            error = new ParseError(ParseErrorType.Directives_EndOfCodeBlockIsunexpected,
                                lines[i].LineIndex, lines[i].FileName);
                            return null;
                        }

                        currentLevel--;
                    }
                }
                else
                {
                    if (startRequired)
                    {
                        error = new ParseError(ParseErrorType.Directives_StartOfCodeBlockExpected,
                            lines[i].LineIndex, lines[i].FileName);
                        return null;
                    }

                    if (isSeekingEnd)
                        continue;

                    if(!lines[i].IsEmpty)
                        block.RawLines.Add(lines[i]);
                }
            }

            if (startRequired)
            {
                error = new ParseError(ParseErrorType.Directives_StartOfCodeBlockExpected,
                    lines[endIndex - 1].LineIndex, lines[endIndex - 1].FileName);
                return null;
            }

            if(isSeekingEnd)
            {
                error = new ParseError(ParseErrorType.Directives_EndOfCodeBlockExpected,
                    lines[endIndex - 1].LineIndex, lines[endIndex - 1].FileName);
                return null;
            }

            error = null;
            return block;
        }

        public CodeBlock ResolveStructures(List<SourceLine> lines, out ParseError error)
        {
            var block = GetLines(0, lines.Count, lines, out error, null);
            if (error != null) return null;

            if(block.ChildBlocks.Count == 0 && source.Machine.BannedFeatures.HasFlag(HASMMachineBannedFeatures.FileWithoutClasses))
            {
                error = new ParseError(ParseErrorType.Other_DocumentWithNoParentClassIsNotAllowed);
                return null;
            }

            if(block.RawLines.Count != 0 && source.Machine.BannedFeatures.HasFlag(HASMMachineBannedFeatures.FileWithoutClasses))
            {
                error = new ParseError(ParseErrorType.Other_InstructionsInRootOfDocumentIsNotAllowed);
                return null;
            }

            error = null;
            return block;
        }

        protected override void InnerRun()
        {
            var lines = source._lines;

			ParseError error;
			var a = ResolveStructures(lines, out error);
            if (error != null)
            {
                InnerEnd(error);
                return;
            }

            source._parentBlock = a;
            InnerEnd();
        }
    }
}
