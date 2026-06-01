using Todo.Application.Common;
using Todo.Application.DTOs.Tasks;

namespace Todo.Application.Interfaces;

public interface ITaskService
{
    Task<PagedResponse<TaskResponse>> GetAllAsync(Guid userId, TaskQuery query, CancellationToken cancellationToken = default);

    Task<TaskResponse> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);

    Task<TaskResponse> CreateAsync(Guid userId, TaskCreateRequest request, CancellationToken cancellationToken = default);

    Task<TaskResponse> UpdateAsync(Guid userId, Guid id, TaskUpdateRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
