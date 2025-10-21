using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeReport.Application.DTOs
{
	public class ReportDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public DateTime CreatedDate { get; set; }= DateTime.UtcNow;

		public int IncidentId { get; set; }
		public string IncidentName { get; set; }

		public int IncidentTypeId { get; set; }
		public string IncidentTypeName { get; set; }
		public int TotalCount { get; set; }
		public string TimeSinceCreated { get; set; } = string.Empty;

	}
}
