
namespace Bravetech.Report.Models
{
    public class Config
    {
        public string PageSize { get; set; } = "A4";
        public bool Portrait { get; set; } = true;
        public string FooterText { get; set; } = string.Empty;
    }
}