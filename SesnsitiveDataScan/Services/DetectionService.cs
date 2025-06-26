using SesnsitiveDataScan.Models;
using SesnsitiveDataScan.Utilities;
using System.Text;
using System.Text.RegularExpressions;

namespace SesnsitiveDataScan.Services
{
    public static class DetectionService
    {
        /// <summary>
        /// Detects and returns sensitive items in the content (but does NOT redact).
        /// </summary>
        public static List<(string Type, string Value)> DetectSensitiveData(string content)
        {
            var sensitiveDataPatterns = GetPatterns();

            var detectedData = new List<(string Type, string Value)>();

            foreach (var (type, pattern) in sensitiveDataPatterns)
            {
                var matches = Regex.Matches(content, pattern);
                foreach (Match match in matches)
                {
                    detectedData.Add((type, match.Value));
                }
            }
            return detectedData;
        }

        /// <summary>
        /// Detects sensitive data and also redacts the content by masking the values.
        /// </summary>
        public static RedactionResult DetectAndRedactSensitiveData( string content,
                                                                    bool scanEmail,
                                                                    bool scanPhone,
                                                                    bool scanCC,
                                                                    bool scanSSN,
                                                                    bool fullRedact)
        {
            var allPatterns = GetPatterns();
            var enabledPatterns = allPatterns.Where(p =>
                (scanEmail && p.Type == "Email") ||
                (scanPhone && p.Type == "Phone") ||
                (scanCC && p.Type == "Credit Card") ||
                (scanSSN && p.Type == "SSN")
            ).ToList();

            var detectedData = new List<(string Type, string Value)>();
            string redactedContent = content;

            foreach (var (type, pattern) in enabledPatterns)
            {
                redactedContent = Regex.Replace(redactedContent, pattern, match =>
                {
                    var value = match.Value;
                    detectedData.Add((type, value));

                    if (fullRedact)
                        return new string('*', value.Length);

                    return type switch
                    {
                        "Email" => MaskingUtils.MaskEmail(value),
                        "SSN" => MaskingUtils.MaskSSN(value),
                        "Credit Card" => MaskingUtils.MaskCreditCard(value),
                        _ => new string('*', value.Length)
                    };
                });
            }

            return new RedactionResult
            {
                RedactedContent = redactedContent,
                DetectedItems = detectedData
            };
        }

        private static List<(string Type, string Pattern)> GetPatterns()
        {
            return new List<(string Type, string Pattern)>
            {
                ("Email", @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}"),
                ("Phone", @"(?<!\d)(?:\+?\d{1,3}[-.\s]?)?(?:\(?\d{3}\)?[-.\s]?)\d{3}[-.\s]?\d{4}(?!\d)"),
                ("Credit Card", @"\b(?:\d[ -]*?){13,16}\b"),
                ("SSN", @"\b\d{3}-\d{2}-\d{4}\b")
            };
        }
    }
}
