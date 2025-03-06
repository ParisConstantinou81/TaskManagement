using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManagement.API.Data;
using TaskManagement.API.Enums;

namespace TaskManagement.API.Interfaces.Persistence
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> dbSet { get; set; }
        TaskManagementDbContext Context { get; set; }

        public Task<IEnumerable<T>> GetCollectionAsync(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");

        public IQueryable<T> GetQuery(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");

        public Task<(CrudEn, int)> InsertOrUpdate(T entity);
        public void Insert(T entity);

        public void Delete(T entity);
        public void Update(T entity);
        public Task<int> UpdateAsync(T entity);

        public Task<int> ExecuteSqlInterpolatedAsync(FormattableString formattableString);

        IQueryable<T> FromSqlRaw(string sql, params object[] parameters);

        public Task<TOutput?> FindSingle<TOutput>(
            Expression<Func<T, bool>> predicate,
            CancellationToken token);

        public Task<List<TOutput>> FindAll<TOutput>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TOutput>> selector,
            CancellationToken token);

        public Task<List<TOutput>> MapAll<TOutput>(CancellationToken token);
    }
}
