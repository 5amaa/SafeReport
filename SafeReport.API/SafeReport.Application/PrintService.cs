using iTextSharp.text;
using iTextSharp.text.pdf;
using SafeReport.Application.DTOs;
namespace SafeReport.Application
{
    public static class PrintService
    {
        public static byte[] GenerateReportPdf(ReportDto report)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 36, 36, 36, 36);
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            document.Add(new Paragraph("SafeReport - Report Details", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            });

            // Table setup
            PdfPTable table = new PdfPTable(5)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 2, 2, 2, 3, 2 });

            AddHeaderCell(table, "Report Name");
            AddHeaderCell(table, "Incident");
            AddHeaderCell(table, "Incident Type");
            AddHeaderCell(table, "Created Date");
            AddHeaderCell(table, "Address");

            // Single report row
            AddCell(table, report.Description ?? "N/A");
            AddCell(table, report.IncidentName ?? "N/A");
            AddCell(table, report.IncidentTypeName ?? "N/A");
            AddCell(table, report.CreatedDate.ToString("yyyy-MM-dd HH:mm"));
            AddCell(table, report.Address ?? "N/A");

            document.Add(table);
            document.Close();

            return memoryStream.ToArray();
        }

        // Helper methods remain the same
        private static void AddHeaderCell(PdfPTable table, string text)
        {
            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.White);
            var cell = new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = new BaseColor(0, 102, 204),
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 8
            };
            table.AddCell(cell);
        }

        private static void AddCell(PdfPTable table, string text)
        {
            var font = FontFactory.GetFont(FontFactory.HELVETICA, 11);
            var cell = new PdfPCell(new Phrase(text, font))
            {
                Padding = 6
            };
            table.AddCell(cell);
        }
    }
}
