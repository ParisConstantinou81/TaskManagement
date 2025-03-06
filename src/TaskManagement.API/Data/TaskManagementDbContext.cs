using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data.Entities.Tables;

namespace TaskManagement.API.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : base(options) { }

        public DbSet<TbTask> Tasks { get; set; }
    }
}
