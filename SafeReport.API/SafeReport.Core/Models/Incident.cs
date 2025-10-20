using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeReport.Core.Models
{
	public class Incident
	{
		public int Id { get; set; }

		public string NameEn { get; set; }
		public string NameAr { get; set; }

		public string? DescriptionEn { get; set; }
		public string? DescriptionAr { get; set; }

		public DateTime CreationDate { get; set; }
		public bool IsDeleted { get; set; }
	}
}
