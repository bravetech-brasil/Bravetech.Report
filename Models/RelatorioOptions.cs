namespace Bravetech.Report.PdfGenerator.Models
{
    public class RelatorioOptions
    {
        public string HeaderText { get; set; } = string.Empty;
        public string FooterText { get; set; } = "V1";
        public bool Portrait { get; set; } = true;
    }
}