using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskManagement.API.Configuration;
using TaskManagement.API.Data;
using TaskManagement.API.Data.Cache;
using TaskManagement.API.Data.Repositories;
using TaskManagement.API.Data.UnitOfWork;
using TaskManagement.API.Interfaces.Persistence;
using TaskManagement.API.Interfaces.Persistence.Cache;
using TaskManagement.API.Interfaces.Services;
using TaskManagement.API.Services;

namespace TaskManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrustructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddMappingService();
            builder.Services.AddCachingService();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
