using Bravetech.Report.PdfGenerator.Interfaces;
using Bravetech.Report.PdfGenerator.Models;
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

        public byte[] GerarPdf(string html, RelatorioOptions relatorioOptions)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pageSize = relatorioOptions.Portrait ? PageSize.A4 : PageSize.A4.Rotate();
            var pdfDoc = new PdfDocument(writer);
            pdfDoc.SetDefaultPageSize(pageSize);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var converterProperties = new ConverterProperties();

            HtmlConverter.ConvertToDocument(html, pdfDoc, converterProperties);

            int totalPages = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= totalPages; i++)
            {
                var page = pdfDoc.GetPage(i);
                var canvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                var pageSizeObj = page.GetPageSize();

                // Header
                canvas.BeginText();
                canvas.SetFontAndSize(font, 12);
                canvas.MoveText(pageSizeObj.GetWidth() / 2 - 50, pageSizeObj.GetTop() - 20);
                canvas.ShowText(relatorioOptions.HeaderText ?? "");
                canvas.EndText();

                // Footer
                canvas.BeginText();
                canvas.SetFontAndSize(font, 10);
                canvas.MoveText(pageSizeObj.GetWidth() / 2 - 30, pageSizeObj.GetBottom() + 20);
                canvas.ShowText($"{relatorioOptions.FooterText ?? ""} - Página {i} de {totalPages}");
                canvas.EndText();
            }

            pdfDoc.Close();
            return ms.ToArray();
        }
    }
}