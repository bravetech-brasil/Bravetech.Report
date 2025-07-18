namespace Bravetech.Report.PdfGenerator.Interfaces
{
    public interface IPdfGeneratorService
    {
        byte[] GerarPdf(string html);
    }
}