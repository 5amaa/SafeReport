using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using SafeReport.Application.Common;
using SafeReport.Application.DTOs;
using SafeReport.Application.Helper;
using SafeReport.Application.ISevices;
using SafeReport.Core.Interfaces;
using SafeReport.Core.Models;
using System.Linq.Expressions;

namespace SafeReport.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IIncidentTypeRepository _incidentTypeRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<ReportHub> _hubContext;
        private readonly IWebHostEnvironment _env;
        public ReportService(
            IReportRepository reportRepository,
            IIncidentTypeRepository incidentTypeRepository,
            IViolationRepository violationRepository,
            IOtherRepository otherRepository,
            IMapper mapper,
            IHubContext<ReportHub> hubContext,
            IWebHostEnvironment env)
        {
            _reportRepository = reportRepository;
            _incidentTypeRepository = incidentTypeRepository;
            _mapper = mapper;
            _hubContext = hubContext;
            _env = env;
        }

        public async Task<Response<PagedResultDto>> GetPaginatedReportsAsync(ReportFilterDto? filter)
        {
            try
            {

                Expression<Func<Report, bool>> predicate = r => true;

                if (filter.IncidentId.HasValue && filter.CreatedDate.HasValue)
                {
                    predicate = r => r.IncidentId == filter.IncidentId.Value && r.CreatedDate.Date == filter.CreatedDate.Value.Date;
                }
                else if (filter.IncidentId.HasValue)
                {
                    predicate = r => r.IncidentId == filter.IncidentId.Value;
                }
                else if (filter.CreatedDate.HasValue)
                {
                    predicate = r => r.CreatedDate.Date == filter.CreatedDate.Value.Date;
                }

                // Pass to repository
                var reports = await _reportRepository.GetPagedAsync(
                    filter.PageNumber.Value,
                    filter.PageSize.Value,
                    predicate);

                var totalCount = await _reportRepository.GetTotalCountAsync();

                // Extract all related incidentTypeIds to fetch them in one go
                var incidentTypeIds = reports.Select(r => r.IncidentTypeId).Distinct().ToList();
                var incidentIds = reports.Select(r => r.IncidentId).Distinct().ToList();

                var incidentTypes = await _incidentTypeRepository
                    .FindAllAsync(t => incidentIds.Contains(t.IncidentId) && incidentTypeIds.Contains(t.Id) && !t.IsDeleted);


                var incidentTypeDict = incidentTypes.ToDictionary(t => (t.IncidentId, t.Id), t => t.NameEn);

                // Map manually or use AutoMapper projection
                var reportDtos = reports.Select(report =>
                {
                    incidentTypeDict.TryGetValue((report.IncidentId, report.IncidentTypeId), out string? incidentTypeName);

                    return new ReportDto
                    {
                        Id = report.Id,
                        Description = report.Description,
                        CreatedDate = report.CreatedDate,
                        IncidentId = report.IncidentId,
                        IncidentName = report.Incident?.NameEn,
                        IncidentTypeId = report.IncidentTypeId,
                        IncidentTypeName = incidentTypeName ?? "N/A",
                        TimeSinceCreated = string.Empty,
                    };
                }).ToList();


                // Wrap in paged result
                var pagedResult = new PagedResultDto
                {
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber.Value,
                    PageSize = filter.PageSize.Value,
                    Reports = reportDtos
                };

                return Response<PagedResultDto>.SuccessResponse(pagedResult, "Fetched reports successfully.");
            }
            catch (Exception ex)
            {
                return Response<PagedResultDto>.FailResponse($"Error: {ex.Message}");
            }
        }
        public async Task<Response<string>> SoftDeleteReportAsync(Guid id)
        {
            try
            {
                var report = await _reportRepository.FindAsync(r => r.Id == id);

                if (report == null)
                    return Response<string>.FailResponse($"Report with ID {id} not found.");

                _reportRepository.SoftDelete(report);
                _reportRepository.SaveChangesAsync();
                return Response<string>.SuccessResponse("Report soft deleted successfully.");
            }
            catch (Exception ex)
            {
                return Response<string>.FailResponse($"Error deleting report: {ex.Message}");
            }
        }
        //public async Task<IActionResult> PrintReports([FromQuery] PaginationFilter filter, int? incidentId, DateTime? dateTime)
        //{
        //	try
        //	{
        //		var result = await GetPaginatedReportsAsync(filter, incidentId, dateTime);

        //		if (result == null || result.Data == null || !result.Data.Reports.Any())
        //			return Response<string>.FailResponse("No reports found to print.");

        //		// Generate the PDF
        //		var pdfBytes = PrintService.GenerateReportsPdf(result.Data.Reports);

        //		// Return as downloadable PDF
        //		return File(pdfBytes, "application/pdf", $"Reports_{DateTime.UtcNow:yyyyMMddHHmm}.pdf");
        //	}
        //	catch (Exception ex)
        //	{
        //		return StatusCode(500, $"Error generating PDF: {ex.Message}");
        //	}
        //}
        public async Task<byte[]> GetReportsPdfAsync(Guid id)
        {
            //var result = await GetPaginatedReportsAsync(filter, incidentId, dateTime);
            Report report = await _reportRepository.FindAsync(r => r.Id == id && !r.IsDeleted);
            if (report == null)
                return null;
            // Map to DTO if necessary


            var reportDto = _mapper.Map<ReportDto>(report);
            return PrintService.GenerateReportPdf(reportDto);
        }
        public async Task<Response<string>> AddReportAsync(CreateReportDto reportDto)
        {
            try
            {
                var report = _mapper.Map<Report>(reportDto);


                if (reportDto.Image != null)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(reportDto.Image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await reportDto.Image.CopyToAsync(stream);

                    report.ImagePath = $"images/{fileName}";
                }


                await _reportRepository.AddAsync(report);
                await _reportRepository.SaveChangesAsync();


                await _hubContext.Clients.All.SendAsync("ReceiveNewReport", reportDto);

                return Response<string>.SuccessResponse("Report added successfully.");
            }
            catch (Exception ex)
            {
                return Response<string>.FailResponse($"Error adding report: {ex.Message}");
            }



        }

        //public async Task<Response<string>> AddReportAsync(ReportDto reportDto)
        //{
        //    try
        //    {
        //        var report = new Report
        //        {
        //            Name = reportDto.Name,
        //            IncidentId = reportDto.IncidentId,
        //            IncidentTypeId = reportDto.IncidentTypeId,
        //            CreatedDate = DateTime.UtcNow
        //        };

        //        await _reportRepository.AddAsync(report);
        //        await _reportRepository.SaveChangesAsync();

        //        await _hubContext.Clients.All.SendAsync("ReceiveNewReport", reportDto);

        //        return Response<string>.SuccessResponse("Report added successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Response<string>.FailResponse($"Error adding report: {ex.Message}");
        //    }
        //}

    }
}





