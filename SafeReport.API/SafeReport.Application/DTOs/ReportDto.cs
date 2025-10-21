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

		public string TimeSinceCreated { get; set; } = string.Empty;
	}
}
