using TaskManagement.API.Data.Entities.Tables;
using TaskManagement.API.Models.DTOs;

namespace TaskManagement.API.Interfaces.Services
{
    public interface ITaskManagementService
    {
        Task<List<TbTask>> GetTasksSorted();
        Task<bool> CreateTask(CreateTaskDto task);
        Task<TbTask> GetTaskById(int id);
        Task<bool> UpdateTask(int id, UpdateTaskDto task);
        Task<bool> DeleteTask(int id);
        Task<bool> BulkUpdateTasks(IEnumerable<int> ids);
    }
}
