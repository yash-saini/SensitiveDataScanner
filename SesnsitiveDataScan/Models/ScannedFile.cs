namespace SesnsitiveDataScan.Models
{
    public class ScannedFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime ScanDate { get; set; }

        public ICollection<FlaggedItem> FlaggedItems { get; set; }
    }
}
