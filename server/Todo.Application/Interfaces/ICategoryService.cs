using Todo.Application.Common;
using Todo.Application.DTOs.Categories;

namespace Todo.Application.Interfaces;

public interface ICategoryService
{
    Task<PagedResponse<CategoryResponse>> GetAllAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<CategoryResponse> CreateAsync(Guid userId, CategoryCreateRequest request, CancellationToken cancellationToken = default);

    Task<CategoryResponse> UpdateAsync(Guid userId, Guid id, CategoryUpdateRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
