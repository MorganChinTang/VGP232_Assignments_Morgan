using System.Collections.Generic;

namespace Assignment3_Morgan
{
    public class SpriteSheetProject
    {
        public string OutputDirectory { get; set; } = string.Empty;
        public string OutputFile { get; set; } = string.Empty;
        public List<string> ImagePaths { get; set; } = new();
        public bool IncludeMetaData { get; set; }
        public int Columns { get; set; } = 1;
    }
}
