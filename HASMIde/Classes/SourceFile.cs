using System;

namespace HASM
{
    public struct SourceFile : ICloneable
    {
        public string DisplayNumber;
        public string Path;

        public SourceFile(string displayNumber, string path)
        {
            DisplayNumber = displayNumber;
            Path = path;
        }

        public object Clone()
        {
            return new SourceFile()
            {
                DisplayNumber = DisplayNumber,
                Path = Path
            };
        }

        public override string ToString()
        {
            return DisplayNumber;
        }
    }
}
