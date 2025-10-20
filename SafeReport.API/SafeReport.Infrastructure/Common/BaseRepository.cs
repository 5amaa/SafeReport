using Microsoft.EntityFrameworkCore;
using SafeReport.Application.Interfaces;
using SafeReport.Infrastructure.Context;

namespace SafeReport.Infrastructure.Common
{
	public class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		protected readonly SafeReportDbContext _context;
		protected readonly DbSet<T> _dbSet;

		public BaseRepository(SafeReportDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}

		public async Task<T?> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void Update(T entity)
		{
			_dbSet.Update(entity);
		}

		public void SoftDelete(T entity)
		{
			var prop = typeof(T).GetProperty("IsDeleted");
			if (prop != null && prop.PropertyType == typeof(bool))
			{
				prop.SetValue(entity, true);
				_dbSet.Update(entity);
			}
			else
			{

				_dbSet.Remove(entity);
			}
		}


		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
