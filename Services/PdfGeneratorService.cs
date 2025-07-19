using Bravetech.Report.Models;
using Bravetech.Report.PdfGenerator.Interfaces;
using iText.Html2pdf;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using System.IO;

namespace Bravetech.Report.PdfGenerator
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public PdfGeneratorService()
        {
        }

        public byte[] GerarPdf(string html, Config relatorio)
        {
            using var ms = new MemoryStream();

            var writer = new PdfWriter(ms);
            var pageSize = GetPaperSize(relatorio.PageSize, relatorio.Portrait);

            var pdfDoc = new PdfDocument(writer);
            pdfDoc.SetDefaultPageSize(pageSize);

            var converterProperties = new ConverterProperties();
            HtmlConverter.ConvertToDocument(html, pdfDoc, converterProperties);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            int totalPages = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= totalPages; i++)
            {
                var page = pdfDoc.GetPage(i);
                var canvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                var pageSizeObj = page.GetPageSize();
                var footer = string.IsNullOrWhiteSpace(relatorio.FooterText)
                    ? $"Página {i} de {totalPages}"
                    : $"{relatorio.FooterText} - Página {i} de {totalPages}";

                canvas.BeginText();
                canvas.SetFontAndSize(font, 10);
                canvas.MoveText(pageSizeObj.GetWidth() / 2 - 60, pageSizeObj.GetBottom() + 20);
                canvas.ShowText(footer);
                canvas.EndText();
            }

            pdfDoc.Close();
            return ms.ToArray();
        }

        private PageSize GetPaperSize(string size, bool retrato)
        {
            size = size?.ToUpperInvariant() ?? "A4";

            var pageSize = size switch
            {
                "A3" => PageSize.A3,
                "A5" => PageSize.A5,
                "LETTER" => PageSize.LETTER,
                "LEGAL" => PageSize.LEGAL,
                "TABLOID" => PageSize.TABLOID,
                _ => PageSize.A4
            };

            return retrato ? pageSize : pageSize.Rotate();
        }
    }
}