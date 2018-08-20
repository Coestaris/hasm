using System;
using System.Collections.Generic;

namespace HASM.Classes
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

        public override bool Equals(object obj)
        {
            return obj is SourceFile file && file.Path.CompareAsPath(Path);
        }

		public override int GetHashCode()
		{
			var hashCode = -1931686138;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayNumber);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
			return hashCode;
		}

		public override string ToString()
        {
            return DisplayNumber;
        }

    }
}
