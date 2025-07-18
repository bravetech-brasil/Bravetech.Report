using Bravetech.Report.Models;

namespace Bravetech.Report.PdfGenerator.Interfaces
{
    public interface IPdfGeneratorService
    {
        byte[] GerarPdf(string html, Relatorio relatorio);
    }
}