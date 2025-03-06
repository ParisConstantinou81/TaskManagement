using TaskManagement.API.Data;

namespace TaskManagement.API.Interfaces.Persistence
{
    public interface IUnitOfWork<T> : IDisposable where T : class
    {
        IRepository<T> Repository { get; set; }
        TaskManagementDbContext Context { get; }
        Task CreateTransaction();
        Task Commit();
        Task Rollback();
    }
}
