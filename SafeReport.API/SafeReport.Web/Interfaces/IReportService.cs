using SafeReport.Web.DTOs;

namespace SafeReport.Web.Interfaces;

public interface IReportService
{
    Task<Response<PagedResultDto>> GetAllReportsAsync(ReportFilterDto filter);
    Task<bool> DeleteReportAsync(int id);
    Task PrintReportAsync(int id);
    //Task<List<Response<IncidentType>>> GetIncidentTypesAsync();
    Task<List<Response<IncidentType>>> GetAllIncidentsAsync();
}
