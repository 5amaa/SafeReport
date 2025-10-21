using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeReport.Application.Common;
using SafeReport.Application.DTOs;
using SafeReport.Application.ISevices;
using SafeReport.Core.Interfaces;

namespace SafeReport.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReportController(IReportService reportService) : ControllerBase
	{
		private readonly IReportService _reportService =  reportService;

	

		[HttpPost("GetAll")]
		public async Task<Response<PagedResultDto>> GetAll(PaginationFilter filter, int? incidentId, DateTime? dateTime)
		{
			var result = await _reportService.GetPaginatedReportsAsync(filter, incidentId, dateTime);


			return Response<PagedResultDto>.SuccessResponse(result.Data, "Reports retrieved successfully");
		}
	}
}
