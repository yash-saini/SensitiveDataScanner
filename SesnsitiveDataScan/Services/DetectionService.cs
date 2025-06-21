namespace SesnsitiveDataScan.Services
{
    public static class DetectionService
    {
        public static List<(string Type, string Value)> DetectSensitiveData(string content)
        {
            var sensitiveDataPatterns = new List<(string Type, string Pattern)>
            {
                ("Email", @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}"),
                ("Phone", @"\+?[0-9\s\-\(\)]+"),
                ("Credit Card", @"\b(?:\d{4}[ -]?){3}\d{4}\b"),
                ("SSN", @"\b\d{3}-\d{2}-\d{4}\b")
            };

            var detectedData = new List<(string Type, string Value)>();

            foreach (var (type, pattern) in sensitiveDataPatterns)
            {
                var matches = System.Text.RegularExpressions.Regex.Matches(content, pattern);
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    detectedData.Add((type, match.Value));
                }
            }

            return detectedData;
        }
    }
}
