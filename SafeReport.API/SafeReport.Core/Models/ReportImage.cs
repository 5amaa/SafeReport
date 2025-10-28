using System.ComponentModel.DataAnnotations.Schema;

namespace SafeReport.Core.Models
{
    [Table("ReportImage")]
    public class ReportImage
    {
        public int Id { get; set; }
        public Guid ReportId { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Report Report { get; set; } = null!;
    }
}
