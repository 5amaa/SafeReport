using Microsoft.EntityFrameworkCore;
using SafeReport.Application.Interfaces;
using SafeReport.Core.Interfaces;
using SafeReport.Core.Models;
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
			if (entity is ISoftDelete softDeletable)
			{
				softDeletable.IsDeleted = true;
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

		public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize)
		{
			IQueryable<T> query = _dbSet;

			if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
			{
				query = query.Where(e => !((ISoftDelete)e).IsDeleted);
			}

			return await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
	}
}
