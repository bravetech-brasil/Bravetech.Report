using Bravetech.Report.Models;
using Bravetech.Report.PdfGenerator.Interfaces;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using System;
using System.Globalization;
using System.IO;

namespace Bravetech.Report.PdfGenerator
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public PdfGeneratorService() { }

        public byte[] GerarPdf(string html, Config relatorio)
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (relatorio == null)
            {
                throw new ArgumentNullException(nameof(relatorio));
            }

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdfDoc = new PdfDocument(writer);

            // Tamanho/orientação
            var pageSize = GetPaperSize(relatorio.PageSize, relatorio.Portrait);
            pdfDoc.SetDefaultPageSize(pageSize);

            // CSS injetado: margens + rodapé com contadores
            var css = BuildInjectedCss(relatorio);

            // Garante que o CSS vai junto (insere no <head> ou cria um <head>)
            string htmlWithCss = InjectCssIntoHtml(html, css);

            // Converter com suporte a fontes e mídia "print"
            var props = new ConverterProperties()
                .SetFontProvider(new DefaultFontProvider(true, true, true))
                .SetMediaDeviceDescription(iText.StyledXmlParser.Css.Media.MediaDeviceDescription.CreateDefault());

            HtmlConverter.ConvertToPdf(htmlWithCss, pdfDoc, props);

            pdfDoc.Close();
            return ms.ToArray();
        }
        private static string BuildInjectedCss(Config relatorio)
        {
            // mm -> CSS direto em mm (pdfHTML entende)
            var top = $"{relatorio.MargemTopo:0.###}mm";
            var right = $"{relatorio.MargemDireita:0.###}mm";
            var bottom = $"{relatorio.MargemInferior:0.###}mm";
            var left = $"{relatorio.MargemEsquerda:0.###}mm";

            // Texto do rodapé
            var prefix = string.IsNullOrWhiteSpace(relatorio.FooterText)
                ? ""
                : relatorio.FooterText.Trim() + " - ";

            var culture = CultureInfo.GetCultureInfo("pt-BR");
            var dataHoraUtc = DateTime.UtcNow.ToString("g", culture) + " UTC";

            return $@"
            <style>
              @page {{
                margin: {top} {right} {bottom} {left};

                @bottom-left {{
                  content: '{EscapeCss(prefix)}{dataHoraUtc}';
                  font-family: Helvetica, Arial, sans-serif;
                  font-size: 10pt;
                }}

                @bottom-right {{
                  content: 'Página ' counter(page) ' de ' counter(pages);
                  font-family: Helvetica, Arial, sans-serif;
                  font-size: 10pt;
                }}
              }}

              body {{
                font-family: Helvetica, Arial, sans-serif;
              }}
            </style>";
        }

        private static string InjectCssIntoHtml(string html, string cssBlock)
        {
            // Insere o CSS antes de </head>. Se não houver <head>, cria um.
            var idx = html.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                return html.Substring(0, idx) + cssBlock + html.Substring(idx);
            }

            // Se não tem <head>, tenta antes do <body>
            var bodyIdx = html.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
            if (bodyIdx >= 0)
            {
                var insertIdx = html.IndexOf(">", bodyIdx, StringComparison.OrdinalIgnoreCase);
                if (insertIdx >= 0) insertIdx++; // após o '>'
                return html.Substring(0, insertIdx)
                     + cssBlock
                     + html.Substring(insertIdx);
            }

            // Último recurso: prefixa o HTML com um <head> contendo o CSS
            return "<head>" + cssBlock + "</head>" + html;
        }

        private static string EscapeCss(string s)
        {
            // Escapa apóstrofos e barras invertidas para CSS dentro de '...'
            return s?.Replace("\\", "\\\\").Replace("'", "\\'") ?? "";
        }

        private PageSize GetPaperSize(string size, bool retrato)
        {
            size = size?.ToUpperInvariant() ?? "A4";
            var pz = size switch
            {
                "A3" => PageSize.A3,
                "A5" => PageSize.A5,
                "LETTER" => PageSize.LETTER,
                "LEGAL" => PageSize.LEGAL,
                "TABLOID" => PageSize.TABLOID,
                _ => PageSize.A4
            };
            return retrato ? pz : pz.Rotate();
        }
    }
}
