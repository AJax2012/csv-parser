namespace GardnerCsvParser.Models
{
    public class UserInputDto
    {
        public string InputFilePath { get; set; }
        public string OutputDirectory { get; set; }
        public char RowSeparator { get; set; }
    }
}
