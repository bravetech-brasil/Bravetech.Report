using Bravetech.Report.Models;
using Bravetech.Report.PdfGenerator.Interfaces;
using iText.Html2pdf;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
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
            var document = new iText.Layout.Document(pdfDoc, pageSize);

            float top = (float)relatorio.MargemTopo * 2.835f;
            float right = (float)relatorio.MargemDireita * 2.835f;
            float bottom = (float)relatorio.MargemInferior * 2.835f;
            float left = (float)relatorio.MargemEsquerda * 2.835f;

            document.SetMargins(top, right, bottom, left);
            var elements = HtmlConverter.ConvertToElements(html, converterProperties);

            foreach (var element in elements)
            {
                document.Add((iText.Layout.Element.IBlockElement)element);
            }

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            int totalPages = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= totalPages; i++)
            {
                var page = pdfDoc.GetPage(i);
                var canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                var pageSizeObj = page.GetPageSize();

                var footer = string.IsNullOrWhiteSpace(relatorio.FooterText)
                    ? $"Página {i} de {totalPages}"
                    : $"{relatorio.FooterText} - Página {i} de {totalPages}";

                float x = (pageSizeObj.GetWidth() - font.GetWidth(footer, 10)) / 2;
                float y = pageSizeObj.GetBottom() + 20;

                canvas.BeginText();
                canvas.SetFontAndSize(font, 10);
                canvas.MoveText(x, y);
                canvas.ShowText(footer);
                canvas.EndText();
            }

            document.Close(); 
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