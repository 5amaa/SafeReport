using SafeReport.Application.Common;
using SafeReport.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeReport.Application.ISevices
{
	public interface IReportService
	{
		//Task<Response<PagedResultDto>> GetPaginatedReportsAsync(PaginationFilter filter);
		Task<Response<PagedResultDto>> GetPaginatedReportsAsync(PaginationFilter filter, int? incidentId, DateTime? dateTime);

		Task<Response<string>> SoftDeleteReportAsync(Guid id);
		Task<byte[]> GetReportsPdfAsync(Guid id);
	}
}
