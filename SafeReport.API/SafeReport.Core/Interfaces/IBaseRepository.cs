using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SafeReport.Application.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<T?> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllAsync();
		Task AddAsync(T entity);
		void Update(T entity);
		void SoftDelete(T entity);
		Task SaveChangesAsync();
	}
}
