using SafeReport.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeReport.Core.Models
{
	[Table("Report")]
	public class Report: ISoftDelete
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public int IncidentId { get; set; }             // Fire / Violation / Other
		public int IncidentTypeId { get; set; }         // Forests / Accidents / etc.
		public bool IsDeleted { get; set; } = false;

		public Incident Incident { get; set; } = null!;
	}
}
