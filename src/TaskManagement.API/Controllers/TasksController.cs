using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Data.Entities.Tables;
using TaskManagement.API.Interfaces.Services;
using TaskManagement.API.Models.DTOs;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    public ITaskManagementService TaskManagementService { get; }
    public IMapper Mapper { get; }

    public TasksController(ITaskManagementService taskManagementService, IMapper mapper)
    {
        TaskManagementService = taskManagementService;
        Mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateTaskDto taskDto)
    {
        if (await TaskManagementService.CreateTask(taskDto))
            return Ok();
        else
            return BadRequest();
    }

    [HttpGet]
    public async Task<IEnumerable<TbTask>> Get()
    {
        return await TaskManagementService.GetTasksSorted();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<TbTask> GetById(int id)
    {
        return await TaskManagementService.GetTaskById(id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromQuery] UpdateTaskDto taskDto)
    {
        if (await TaskManagementService.UpdateTask(id, taskDto))
            return Ok();
        else
            return BadRequest();
    }

    [HttpPut]
    public async Task<ActionResult> Update(IEnumerable<int> ids)
    {
        if (await TaskManagementService.BulkUpdateTasks(ids))
            return Ok();
        else
            return BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (await TaskManagementService.DeleteTask(id))
            return Ok();
        else
            return BadRequest();
    }
}
