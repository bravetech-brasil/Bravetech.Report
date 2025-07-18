using Bravetech.Report.PdfGenerator.Models;

namespace Bravetech.Report.PdfGenerator.Interfaces
{
    public interface IPdfGeneratorService
    {
        byte[] GerarPdf(string html, RelatorioOptions relatorioOptions);
    }
}