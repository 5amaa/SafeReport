using SafeReport.Web.DTOs;

namespace SafeReport.Web.Interfaces;

public interface IReportService
{
    Task<List<ReportDTO>> GetAllReportsAsync();
    Task<bool> DeleteReportAsync(int id);
    Task PrintReportAsync(int id);
}
