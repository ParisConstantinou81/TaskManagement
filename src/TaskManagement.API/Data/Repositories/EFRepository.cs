using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManagement.API.Enums;
using TaskManagement.API.Interfaces.Persistence;

namespace TaskManagement.API.Data.Repositories
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IMapper _mapper;

        public EfRepository(IMapper mapper) => _mapper = mapper;

        public DbSet<TEntity> dbSet { get; set; } = null!;
        public TaskManagementDbContext Context { get; set; } = null!;

        public async Task<IEnumerable<TEntity>> GetCollectionAsync(Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null) query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null) return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity>? query = dbSet;
            if (query == null) return new List<TEntity>().AsQueryable();
            if (filter != null) query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null) return orderBy(query);
            return query;
        }

        public virtual async Task<(CrudEn, int)> InsertOrUpdate(TEntity entity)
        {
            CrudEn enumRes;
            var id = ((dynamic)entity).Id;
            var entityDb = await GetById(id);

            if (entityDb != null)
            {
                ((TaskManagementDbContext)Context).Entry<TEntity>(entityDb).State = EntityState.Detached;
                enumRes = CrudEn.Update;
                Update(entity);
            }
            else
            {
                enumRes = CrudEn.Create;
                Insert(entity);
            }

            return (enumRes, await ((TaskManagementDbContext)Context).SaveChangesAsync());
        }

        public void Update(TEntity entity)
        {
            dbSet.Attach(entity);
            var entry = ((TaskManagementDbContext)Context).Entry(entity);
            entry.State = EntityState.Modified;
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            if (entity == null) return -1;
            dbSet.Attach(entity);
            var entry = ((TaskManagementDbContext)Context).Entry(entity);
            entry.State = EntityState.Modified;
            return await ((TaskManagementDbContext)Context).SaveChangesAsync();
        }

        public virtual void Insert(TEntity entity) => dbSet.AddAsync(entity);

        public virtual void Delete(TEntity entity) => dbSet.Remove(entity);

        public Task<int> ExecuteSqlInterpolatedAsync(FormattableString formattableString) =>
            ((TaskManagementDbContext)Context).Database.ExecuteSqlAsync(formattableString);

        public IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters) =>
            dbSet.FromSqlRaw(sql, parameters);

        public Task<TOutput?> FindSingle<TOutput>(Expression<Func<TEntity, bool>> predicate, CancellationToken token) =>
            dbSet.Where(predicate).ProjectTo<TOutput>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(token);

        public Task<List<TOutput>> FindAll<TOutput>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TOutput>> selector, CancellationToken token) =>
            dbSet.Where(predicate).Select(selector).ToListAsync(token);

        public Task<List<TOutput>> MapAll<TOutput>(CancellationToken token) =>
            dbSet.ProjectTo<TOutput>(_mapper.ConfigurationProvider).ToListAsync(token);

        public async Task<TEntity> GetById(object id) => (await dbSet.FindAsync(id))!;
    }
}
