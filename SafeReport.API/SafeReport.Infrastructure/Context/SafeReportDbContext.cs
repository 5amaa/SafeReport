using Microsoft.EntityFrameworkCore;
using SafeReport.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeReport.Infrastructure.Context
{
	public class SafeReportDbContext(DbContextOptions<SafeReportDbContext> options) : DbContext(options)
	{
		public DbSet<Incident> Incidents { get; set; }
		public DbSet<IncidentType> IncidentTypes { get; set; }
		public DbSet<ViolationIncident> ViolationIncidents { get; set; }
		public DbSet<OtherIncident> OtherIncidents { get; set; }
		public DbSet<Report> Reports { get; set; }
	}

}
