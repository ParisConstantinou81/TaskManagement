using AutoMapper;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TaskManagement.API.Data.Repositories;
using TaskManagement.API.Interfaces.Persistence;

namespace TaskManagement.API.Data.UnitOfWork
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly Dictionary<Type, object> _repositories = default!;
        private readonly IMapper mapper;
        private IDbContextTransaction? contextTran;
        private bool disposedValue;
        private IRepository<T> repository;

        public UnitOfWork(TaskManagementDbContext context, IRepository<T> repo, IMapper map)
        {
            Context = context;
            repository = repo ?? throw new ArgumentNullException(nameof(repo));
            mapper = map;
            repository.Context = context;
            repo.dbSet = ((TaskManagementDbContext)context).Set<T>();
        }

        public DatabaseFacade DatabaseFD => ((TaskManagementDbContext)Context).Database;

        public IRepository<T> Repository
        {
            get => repository;
            set => _ = value;
        }

        public TaskManagementDbContext Context { get; }

        public async Task CreateTransaction()
        {
            contextTran?.Dispose();
            contextTran = await DatabaseFD.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            if (contextTran != null)
                await contextTran.CommitAsync();
        }

        public async Task Rollback()
        {
            if (contextTran != null)
            {
                await contextTran.RollbackAsync();
                await contextTran.DisposeAsync();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
                return (IRepository<TEntity>)_repositories[typeof(TEntity)];

            var repo = new EfRepository<TEntity>(mapper);
            _repositories.Add(typeof(TEntity), repo);
            return repo;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    contextTran?.Dispose();
                    repository = null!;
                }

                disposedValue = true;
            }
        }

        ~UnitOfWork() => Dispose(false);
    }
}
