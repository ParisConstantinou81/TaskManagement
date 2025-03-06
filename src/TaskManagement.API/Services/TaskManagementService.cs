using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data.Entities.Tables;
using TaskManagement.API.Enums;
using TaskManagement.API.Interfaces.Persistence;
using TaskManagement.API.Interfaces.Persistence.Cache;
using TaskManagement.API.Interfaces.Services;
using TaskManagement.API.Models.DTOs;

namespace TaskManagement.API.Services
{
    public class TaskManagementService : ITaskManagementService
    {
        private readonly IUnitOfWork<TbTask> UnitOfWork;
        private readonly IMapper Mapper;
        private readonly ICacheService CacheService;

        public TaskManagementService(IUnitOfWork<TbTask> unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            CacheService = cacheService;
        }

        public async Task<bool> CreateTask(CreateTaskDto taskDto)
        {

            var task = Mapper.Map<TbTask>(taskDto);

            SetTaskPriority(task);
            SetTaskStatus(task);

            UnitOfWork.Repository.Insert(task);
            await UnitOfWork.Repository.Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTask(int id)
        {
            var _task = await UnitOfWork.Repository.GetQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (_task == null)
                return false;

            UnitOfWork.Repository.Delete(_task);
            await UnitOfWork.Repository.Context.SaveChangesAsync();
            return true;
        }

        public async Task<TbTask> GetTaskById(int id)
        {
            return await CacheService.GetOrCreateAsync(string.Format("Task-{0}", id), async () => await UnitOfWork.Repository.GetQuery(x => x.Id == id).FirstOrDefaultAsync());
        }

        public async Task<List<TbTask>> GetTasksSorted()
        {
            var tasks = await UnitOfWork.Repository.GetCollectionAsync();
            return tasks.OrderBy(x => x.Priority).ToList();
        }

        public async Task<bool> UpdateTask(int id, UpdateTaskDto taskDto)
        {
            var task = await UnitOfWork.Repository.GetQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (task == null)
                return false;

            var taskMapped = Mapper.Map<UpdateTaskDto, TbTask>(taskDto, task);

            if (SetTaskStatus(taskMapped))
            {
                await UnitOfWork.Repository.InsertOrUpdate(task);
                return true;
            }

            return false;
        }

        public async Task<bool> BulkUpdateTasks(IEnumerable<int> ids)
        {
            var tasks = await UnitOfWork.Repository.GetQuery(x => ids.Contains(x.Id)).ToListAsync();

            await UnitOfWork.CreateTransaction();

            try
            {
                foreach (var task in tasks)
                {
                    UpdateTask(task);

                    UnitOfWork.Repository.Update(task);
                    await UnitOfWork.Repository.Context.SaveChangesAsync();
                }

                await UnitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                await UnitOfWork.Rollback();
                return false;
            }
        }

        public void UpdateTask(TbTask task)
        {
            SetTaskStatus(task);
            SetTaskPriority(task);
        }

        private bool SetTaskStatus(TbTask task)
        {
            if (task.Status == TaskStatusEn.Completed && task.DueDate >= DateTime.Now.AddDays(3))
                return false;

            switch (task.Status)
            {
                case TaskStatusEn.Completed:
                    task.Completed = true;
                    task.Pending = false;
                    task.InProgress = false;
                    break;
                case TaskStatusEn.InProgress:
                    task.InProgress = true;
                    task.Completed = false;
                    task.Pending = false;
                    break;
                case TaskStatusEn.Pending:
                    task.Pending = true;
                    task.InProgress = false;
                    task.Completed = false;
                    break;
                default:
                    break;
            }
            return true;
        }

        private void SetTaskPriority(TbTask task)
        {
            if (task.DueDate >= DateTime.Now) // If a task is overdue, it automatically becomes "Urgent".
                task.Priority = (int)TaskUrgencyEn.Urgent;
            else if (task.DueDate.AddDays(1) >= DateTime.Now) // Urgent Tasks: Due within 24 hours.
                task.Priority = (int)TaskUrgencyEn.Urgent;
            else if (task.DueDate.AddDays(3) <= DateTime.Now) // Low Priority Tasks: Due in more than 3 days.
                task.Priority = (int)TaskUrgencyEn.Low;
            else task.Priority = (int)TaskUrgencyEn.Normal; // Normal Tasks: Due within 2-3 days.
        }
    }
}
