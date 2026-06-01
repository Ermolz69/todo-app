using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Extensions;
using Todo.Application.Common;
using Todo.Application.DTOs.Tasks;
using Todo.Application.Interfaces;

namespace Todo.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TaskResponse>>> GetAll([FromQuery] TaskQuery query, CancellationToken cancellationToken)
    {
        return Ok(await _taskService.GetAllAsync(User.GetUserId(), query, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _taskService.GetByIdAsync(User.GetUserId(), id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(TaskCreateRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateAsync(User.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(Guid id, TaskUpdateRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _taskService.UpdateAsync(User.GetUserId(), id, request, cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _taskService.DeleteAsync(User.GetUserId(), id, cancellationToken);
        return NoContent();
    }
}
