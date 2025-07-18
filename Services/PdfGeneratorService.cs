using Bravetech.Report.PdfGenerator.Interfaces;
using Bravetech.Report.PdfGenerator.Models;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Options;
using System.IO;

namespace Bravetech.Report.PdfGenerator
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private readonly RelatorioOptions _options;

        public PdfGeneratorService(IOptions<RelatorioOptions> options)
        {
            _options = options.Value;
        }

        public byte[] GerarPdf(string html)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pageSize = _options.Portrait ? PageSize.A4 : PageSize.A4.Rotate();
            var pdfDoc = new PdfDocument(writer);
            pdfDoc.SetDefaultPageSize(pageSize);

            var fontProvider = new DefaultFontProvider(false, false, false);
            var fontPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "fonts", "Inter-Regular.ttf");

            PdfFont interFont = null;
            interFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

            if (File.Exists(fontPath))
            {
                fontProvider.AddFont(fontPath);
                interFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
            }
            else
            {
                interFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            }

            var converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(fontProvider);
            converterProperties.SetCharset("utf-8");

            var document = HtmlConverter.ConvertToDocument(html, pdfDoc, converterProperties);

            int totalPages = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= totalPages; i++)
            {
                var page = pdfDoc.GetPage(i);
                var canvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                var pageSizeObj = page.GetPageSize();

                canvas.BeginText();
                canvas.SetFontAndSize(interFont, 12);
                canvas.MoveText(pageSizeObj.GetWidth() / 2 - 50, pageSizeObj.GetTop() - 20);
                canvas.ShowText(_options.HeaderText);
                canvas.EndText();

                canvas.BeginText();
                canvas.SetFontAndSize(interFont, 10);
                canvas.MoveText(pageSizeObj.GetWidth() / 2 - 30, pageSizeObj.GetBottom() + 20);
                canvas.ShowText($"{_options.FooterText} - Página {i} de {totalPages}");
                canvas.EndText();
            }

            document.Close();
            return ms.ToArray();
        }
    }
}