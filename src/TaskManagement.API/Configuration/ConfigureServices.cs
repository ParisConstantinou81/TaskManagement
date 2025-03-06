using TaskManagement.API.Data.Cache;
using TaskManagement.API.Data.Repositories;
using TaskManagement.API.Data.UnitOfWork;
using TaskManagement.API.Interfaces.Persistence.Cache;
using TaskManagement.API.Interfaces.Persistence;
using TaskManagement.API.Interfaces.Services;
using TaskManagement.API.Services;
using System.Reflection;
using TaskManagement.API.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.API.Configuration
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(ITaskManagementService), typeof(TaskManagementService));
            return services;
        }

        public static IServiceCollection AddInfrustructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TaskManagementDbContext>(options =>
            {
                //options.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning));
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddTransient(typeof(IRepository<>), typeof(EfRepository<>));
            return services;
        }

        public static IServiceCollection AddMappingService(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }

        public static IServiceCollection AddCachingService(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, CacheService>();
            services.AddDistributedMemoryCache();
            return services;
        }
    }
}
