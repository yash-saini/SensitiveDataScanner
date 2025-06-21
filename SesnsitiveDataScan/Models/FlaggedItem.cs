namespace SesnsitiveDataScan.Models
{
    public class FlaggedItem
    {
        public int Id { get; set; }
        public string Type { get; set; } 
        public string Value { get; set; }
        public int ScannedFileId { get; set; }

        public ScannedFile ScannedFile { get; set; }
    }
}
