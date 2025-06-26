using DocumentFormat.OpenXml.Packaging;

namespace SesnsitiveDataScan.Utilities
{
    public static class WordTextExtractorUtility
    {
        public static string ExtractText(byte[] fileData)
        {
            using var ms = new MemoryStream(fileData);
            using var wordDoc = WordprocessingDocument.Open(ms, false);
            var body = wordDoc.MainDocumentPart.Document.Body;
            return body?.InnerText ?? string.Empty;
        }
    }
}
