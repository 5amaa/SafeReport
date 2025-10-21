using SafeReport.Web.DTOs;
using SafeReport.Web.Interfaces;
using System.Collections.Generic;


namespace SafeReport.Web.Services;

public class ReportService: IReportService
{
    private readonly HttpClient _http;

    public ReportService(HttpClient http)
    {
        _http = http;
    }

    public async Task<Response<PagedResultDto>> GetAllReportsAsync(ReportFilterDto filter)
    {
        await Task.Delay(500); // simulate API call
        var now = DateTime.UtcNow;
        var allReports = new List<ReportDTO>
        {
             new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 1",
            CreatedDate = now.AddHours(-5),
            IncidentId = 1,
            IncidentName = "Fire",
            IncidentTypeId = 2,
            IncidentTypeName = "Forests",
            TimeSinceCreated = "5 ساعات"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 2",
            CreatedDate = now.AddDays(-1),
            IncidentId = 2,
            IncidentName = "Assault",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "1 يوم"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 3",
            CreatedDate = now.AddDays(-3),
            IncidentId = 3,
            IncidentName = "Theft",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "3 أيام"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 2",
            CreatedDate = now.AddDays(-1),
            IncidentId = 2,
            IncidentName = "Assault",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "1 يوم"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 3",
            CreatedDate = now.AddDays(-3),
            IncidentId = 3,
            IncidentName = "Theft",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "3 أيام"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 2",
            CreatedDate = now.AddDays(-1),
            IncidentId = 2,
            IncidentName = "Assault",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "1 يوم"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 3",
            CreatedDate = now.AddDays(-3),
            IncidentId = 3,
            IncidentName = "Theft",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "3 أيام"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 2",
            CreatedDate = now.AddDays(-1),
            IncidentId = 2,
            IncidentName = "Assault",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "1 يوم"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 3",
            CreatedDate = now.AddDays(-3),
            IncidentId = 3,
            IncidentName = "Theft",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "3 أيام"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 2",
            CreatedDate = now.AddDays(-1),
            IncidentId = 2,
            IncidentName = "Assault",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "1 يوم"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 3",
            CreatedDate = now.AddDays(-3),
            IncidentId = 3,
            IncidentName = "Theft",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "3 أيام"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 2",
            CreatedDate = now.AddDays(-1),
            IncidentId = 2,
            IncidentName = "Assault",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "1 يوم"
        },
        new ReportDTO
        {
            Id = Guid.NewGuid(),
            Name = "Sample Report 3",
            CreatedDate = now.AddDays(-3),
            IncidentId = 3,
            IncidentName = "Theft",
            IncidentTypeId = 1,
            IncidentTypeName = "Urban",
            TimeSinceCreated = "3 أيام"
        }
        };
        // Apply filtering if needed
        var filteredReports = allReports.AsEnumerable();
        if (filter.IncidentId.HasValue)
            filteredReports = filteredReports.Where(r => r.IncidentId == filter.IncidentId.Value);
        if (filter.CreatedDate.HasValue)
            filteredReports = filteredReports.Where(r => r.CreatedDate.Date == filter.CreatedDate.Value.Date);

        var totalCount = filteredReports.Count();

        // Apply pagination
        var pagedReports = filteredReports
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        var pagedResult = new PagedResultDto
        {
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            Reports = pagedReports
        };

        return Response<PagedResultDto>.SuccessResponse(pagedResult);
    }

    public Task<List<Response<IncidentType>>> GetIncidentTypesAsync()
        {
            var incidentTypes = new List<IncidentType>
            {
                new IncidentType { Id = 1, Name = "Fire" },
                new IncidentType { Id = 2, Name = "Assault" },
                new IncidentType { Id = 3, Name = "Theft" },
                new IncidentType { Id = 4, Name = "Medical Emergency" },
                new IncidentType { Id = 5, Name = "Other" }
            };

            // Wrap each incident type in a Response<IncidentType>
            var responseList = incidentTypes
                .Select(it => Response<IncidentType>.SuccessResponse(it))
                .ToList();

            return Task.FromResult(responseList);
        }

        public async Task<bool> DeleteReportAsync(int id)
    {
        await Task.Delay(200); // delay وهمي
        return true; // كأن الحذف تم بنجاح
    }

    public async Task PrintReportAsync(int id)
    {
        await Task.Delay(300); // delay وهمي
        Console.WriteLine($"Printing report #{id}...");
    }
}
