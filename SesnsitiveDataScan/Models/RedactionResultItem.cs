namespace SesnsitiveDataScan.Models
{
    public class RedactionResult
    {
        public string RedactedContent { get; set; } = string.Empty;
        public List<(string Type, string Value)> DetectedItems { get; set; } = new();
    }
}
