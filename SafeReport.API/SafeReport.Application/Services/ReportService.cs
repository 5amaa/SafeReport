using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SafeReport.Application.Common;
using SafeReport.Application.DTOs;
using SafeReport.Application.ISevices;
using SafeReport.Core.Interfaces;
using SafeReport.Core.Models;
using SafeReport.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

	}
}




//using AutoMapper;
//using SafeReport.Application.Common;
//using SafeReport.Application.DTOs;
//using SafeReport.Application.ISevices;
//using SafeReport.Core.Interfaces;
//using SafeReport.Core.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SafeReport.Application.Services
//{
//	public class ReportService : IReportService
//	{
//		private readonly IReportRepository _reportRepository;
//		private readonly IIncidentTypeRepository _incidentTypeRepository;
//		private readonly IMapper _mapper;

//		public ReportService(
//			IReportRepository reportRepository,
//			IIncidentTypeRepository incidentTypeRepository,
//			IViolationRepository violationRepository,
//			IOtherRepository otherRepository,
//			IMapper mapper)
//		{
//			_reportRepository = reportRepository;
//			_incidentTypeRepository = incidentTypeRepository;
//			_mapper = mapper;
//		}

//		public async Task<Response<PagedResultDto>> GetAllAsync(PaginationFilter filter)
//		{
//			try
//			{
//				// Get paged data + total count
//				var reports = await _reportRepository.GetPagedAsync(filter.PageNumber, filter.PageSize);
//				var totalCount = await _reportRepository.GetTotalCountAsync();

//				// Convert to list once for performance
//				var reportList = reports.ToList();

//				// Fetch all IncidentTypeIds grouped by IncidentId
//				var fireTypeIds = reportList.Where(r => r.IncidentId == 1).Select(r => r.IncidentTypeId).ToList();
//				var violationTypeIds = reportList.Where(r => r.IncidentId == 2).Select(r => r.IncidentTypeId).ToList();
//				var otherTypeIds = reportList.Where(r => r.IncidentId == 3).Select(r => r.IncidentTypeId).ToList();

//				// Fetch related data once
//				var fireDict = (await _fireRepository.GetAllAsync())
//					.Where(f => fireTypeIds.Contains(f.Id))
//					.ToDictionary(f => f.Id, f => f.NameEn);

//				var violationDict = (await _violationRepository.GetAllAsync())
//					.Where(v => violationTypeIds.Contains(v.Id))
//					.ToDictionary(v => v.Id, v => v.NameEn);

//				var otherDict = (await _otherRepository.GetAllAsync())
//					.Where(o => otherTypeIds.Contains(o.Id))
//					.ToDictionary(o => o.Id, o => o.NameEn);

//				// Map reports to DTOs
//				var reportDtos = _mapper.Map<List<ReportDto>>(reportList);

//				// Fill incident type names
//				foreach (var dto in reportDtos)
//				{
//					string incidentTypeName = dto.IncidentId switch
//					{
//						1 => fireDict.TryGetValue(dto.IncidentTypeId, out var fName) ? fName : "N/A",
//						2 => violationDict.TryGetValue(dto.IncidentTypeId, out var vName) ? vName : "N/A",
//						3 => otherDict.TryGetValue(dto.IncidentTypeId, out var oName) ? oName : "N/A",
//						_ => "Unknown"
//					};

//					dto.IncidentTypeName = incidentTypeName;
//				}

//				var pagedResult = new PagedResultDto
//				{
//					TotalCount = totalCount,
//					PageNumber = filter.PageNumber,
//					PageSize = filter.PageSize,
//					Reports = reportDtos
//				};

//				return Response<PagedResultDto>.SuccessResponse(pagedResult, "Fetched reports successfully.");
//			}
//			catch (Exception ex)
//			{
//				return Response<PagedResultDto>.FailResponse($"Error: {ex.Message}");
//			}
//		}
//	}
//}

