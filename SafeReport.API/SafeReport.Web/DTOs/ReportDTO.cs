namespace SafeReport.Web.DTOs;

public class ReportDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }

    public int IncidentId { get; set; }
    public string IncidentName { get; set; } = string.Empty;

    public int IncidentTypeId { get; set; }
    public string IncidentTypeName { get; set; } = string.Empty;

    public string TimeSinceCreated { get; set; } = string.Empty;
}
