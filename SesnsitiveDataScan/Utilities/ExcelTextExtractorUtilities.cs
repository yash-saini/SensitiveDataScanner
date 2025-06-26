using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;

namespace SesnsitiveDataScan.Utilities
{
    public static class ExcelTextExtractorUtility
    {
        public static string ExtractText(byte[] fileData)
        {
            var sb = new StringBuilder();

            using var ms = new MemoryStream(fileData);
            using var doc = SpreadsheetDocument.Open(ms, false);
            var workbookPart = doc.WorkbookPart;

            foreach (var sheet in workbookPart.Workbook.Sheets.OfType<Sheet>())
            {
                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                var rows = worksheetPart.Worksheet.Descendants<Row>();

                foreach (var row in rows)
                {
                    foreach (var cell in row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
                    {
                        string value = GetCellValue(workbookPart, cell);
                        sb.Append(value + "\t");
                    }
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static string GetCellValue(WorkbookPart workbookPart, DocumentFormat.OpenXml.Spreadsheet.Cell cell)
        {
            if (cell.DataType?.Value == CellValues.SharedString)
            {
                var sst = workbookPart.SharedStringTablePart.SharedStringTable;
                return sst.ChildElements[int.Parse(cell.CellValue.Text)].InnerText;
            }
            return cell.CellValue?.Text ?? string.Empty;
        }
    }
}
