namespace HASM
{
    public struct SourceFile
    {
        public string DisplayNumber;
        public string Path;

        public SourceFile(string displayNumber, string path)
        {
            DisplayNumber = displayNumber;
            Path = path;
        }

        public override string ToString()
        {
            return DisplayNumber;
        }
    }
}
