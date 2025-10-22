using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using SafeReport.Application.Common;
using SafeReport.Application.DTOs;
using SafeReport.Application.ISevices;
using SafeReport.Core.Interfaces;
using SafeReport.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SafeReport.Application.Services
{
	public class ReportService : IReportService
	{
		private readonly IReportRepository _reportRepository;
		private readonly IIncidentTypeRepository _incidentTypeRepository;
		private readonly IMapper _mapper;

		public ReportService(
			IReportRepository reportRepository,
			IIncidentTypeRepository incidentTypeRepository,
			IViolationRepository violationRepository,
			IOtherRepository otherRepository,
			IMapper mapper)
		{
			_reportRepository = reportRepository;
			_incidentTypeRepository = incidentTypeRepository;
			_mapper = mapper;
		}

		public async Task<Response<PagedResultDto>> GetPaginatedReportsAsync(PaginationFilter filter, int? incidentId, DateTime? dateTime)
		{
			try
			{

				Expression<Func<Report, bool>> predicate = r => true;

				if (incidentId.HasValue && dateTime.HasValue)
				{
					predicate = r => r.IncidentId == incidentId.Value && r.CreatedDate.Date == dateTime.Value.Date;
				}
				else if (incidentId.HasValue)
				{
					predicate = r => r.IncidentId == incidentId.Value;
				}
				else if (dateTime.HasValue)
				{
					predicate = r => r.CreatedDate.Date == dateTime.Value.Date;
				}

				// Pass to repository
				var reports = await _reportRepository.GetPagedAsync(
					filter.PageNumber,
					filter.PageSize,
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
						Name = report.Name,
						CreatedDate = report.CreatedDate,
						IncidentId = report.IncidentId,
						IncidentName = report.Incident?.NameEn,
						IncidentTypeId = report.IncidentTypeId,
						IncidentTypeName = incidentTypeName ?? "N/A",
						TimeSinceCreated = (DateTime.UtcNow - report.CreatedDate).TotalDays >= 1
							? $"{(int)(DateTime.UtcNow - report.CreatedDate).TotalDays} يوم"
							: $"{(int)(DateTime.UtcNow - report.CreatedDate).TotalHours} ساعة"
					};
				}).ToList();


				// Wrap in paged result
				var pagedResult = new PagedResultDto
				{
					TotalCount = totalCount,
					PageNumber = filter.PageNumber,
					PageSize = filter.PageSize,
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

	        var reportDto = new ReportDto
	        {
	        	Id = report.Id,
	        	Name = report.Name,
	        	IncidentId = report.IncidentId,
	        	IncidentName = report.Incident?.NameEn,
	        	IncidentTypeId = report.IncidentTypeId,
	        	IncidentTypeName = "fff",
	        	CreatedDate = report.CreatedDate,
	        	TimeSinceCreated = (DateTime.UtcNow - report.CreatedDate).TotalDays >= 1
	        		? $"{(int)(DateTime.UtcNow - report.CreatedDate).TotalDays} يوم"
	        		: $"{(int)(DateTime.UtcNow - report.CreatedDate).TotalHours} ساعة"
	        };

			return PrintService.GenerateReportPdf(reportDto);
		}

	}
}





