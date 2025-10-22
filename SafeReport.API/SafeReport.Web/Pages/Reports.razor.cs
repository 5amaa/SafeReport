using Microsoft.AspNetCore.Components;
using SafeReport.Web.DTOs;
using SafeReport.Web.Services;

namespace SafeReport.Web.Pages;

public partial class Reports
{
    private List<ReportDTO> pagedReports = new();
    private List<IncidentType> reportTypes = new();

    private int? filterType;
    private DateTime? filterDate;

    private int currentPage = 1;
    private int pageSize = 2;
    private int totalPages ;

    [Inject]
    private ReportService ReportService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var typesData = await ReportService.GetAllIncidentsAsync();
        reportTypes = typesData.Select(r => r.Data!).ToList();

        await LoadReportsAsync();
    }

    private async Task LoadReportsAsync()
    {
        var filter = new ReportFilterDto
        {
            IncidentId = filterType,
            CreatedDate = filterDate,
            PageNumber = currentPage,
            PageSize = pageSize
        };

        var response = await ReportService.GetAllReportsAsync(filter);
        if (response.Success && response.Data != null)
        {
            pagedReports = response.Data.Reports.ToList();
            totalPages = (int)Math.Ceiling((double)response.Data.TotalCount / pageSize);
        }
        else
        {
            pagedReports.Clear();
            totalPages = 1;
        }
    }

    private async Task ApplyFilters()
    {
        currentPage = 1;
        await LoadReportsAsync();
    }

    private async Task ResetFilters()
    {
        filterType = null;
        filterDate = null;
        currentPage = 1;
        await LoadReportsAsync();
    }



    private async Task PrevPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            await LoadReportsAsync();
        }
    }

    private async Task NextPage()
    {
        if (currentPage < totalPages)
        {
            currentPage++;
            await LoadReportsAsync();
        }
    }

    private async Task GoToPage(int page)
    {
        if (page >= 1 && page <= totalPages)
        {
            currentPage = page;
            await LoadReportsAsync();
        }
    }

    private async Task PrintReport(Guid id)
    {
        // await ReportService.PrintReportAsync(id);
    }

    private async Task DeleteReport(Guid id)
    {
        // var success = await ReportService.DeleteReportAsync(id);
        // if (success) await LoadReportsAsync();
    }
}
