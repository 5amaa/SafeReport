namespace SafeReport.Web.DTOs
{
    public class IncidentDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public DateTime Creationdate { get; set; }
    }
}
