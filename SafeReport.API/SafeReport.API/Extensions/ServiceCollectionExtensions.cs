using Microsoft.EntityFrameworkCore;
using SafeReport.Core.Interfaces;
using SafeReport.Infrastructure.Context;
using SafeReport.Infrastructure.Repositories;

namespace SafeReport.API.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
		{
			services.AddDbContext<SafeReportDbContext>(options =>
				options.UseSqlServer(connectionString));

			services.AddScoped<IIncidentRepository, IncidentRepository>();
		    services.AddScoped<IFireRepository, FireRepository>();
		    services.AddScoped<IViolationRepository, ViolationRepository>();
		    services.AddScoped<IOtherRepository, OtherIncidentRepository>();

			return services;
		}
	}
}
