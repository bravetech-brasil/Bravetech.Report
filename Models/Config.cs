
namespace Bravetech.Report.Models
{
    public class Config
    {
        public string PageSize { get; set; } = "A4";
        public bool Portrait { get; set; } = true;
        public string FooterText { get; set; } = string.Empty;
        public decimal MargemTopo { get; set; } = 10;
        public decimal MargemInferior { get; set; } = 10;
        public decimal MargemEsquerda { get; set; } = 10;
        public decimal MargemDireita { get; set; } = 10;
    }
}