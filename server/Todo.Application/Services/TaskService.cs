using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.DTOs.Tasks;
using Todo.Application.Exceptions;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Services;

public class TaskService : ITaskService
{
    private readonly IApplicationDbContext _context;

    public TaskService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<TaskResponse>> GetAllAsync(Guid userId, TaskQuery query, CancellationToken cancellationToken = default)
    {
        var page = NormalizePage(query.Page);
        var pageSize = NormalizePageSize(query.PageSize);

        IQueryable<TaskItem> tasks = _context.TaskItems
            .AsNoTracking()
            .Include(task => task.Category)
            .Where(task => task.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            tasks = tasks.Where(task =>
                task.Title.Contains(search) ||
                (task.Description != null && task.Description.Contains(search)));
        }

        if (query.CategoryId.HasValue)
        {
            tasks = tasks.Where(task => task.CategoryId == query.CategoryId);
        }

        if (query.IsCompleted.HasValue)
        {
            tasks = tasks.Where(task => task.IsCompleted == query.IsCompleted);
        }

        tasks = tasks.OrderBy(task => task.SortOrder).ThenByDescending(task => task.CreatedAt);

        var totalCount = await tasks.CountAsync(cancellationToken);
        var items = await tasks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(task => Map(task))
            .ToListAsync(cancellationToken);

        return new PagedResponse<TaskResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<TaskResponse> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _context.TaskItems
            .AsNoTracking()
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);

        return task is null ? throw new NotFoundException("Task not found") : Map(task);
    }

    public async Task<TaskResponse> CreateAsync(Guid userId, TaskCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateTitle(request.Title);
        await EnsureOwnedCategoryAsync(userId, request.CategoryId, cancellationToken);

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            SortOrder = request.SortOrder,
            UserId = userId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(userId, task.Id, cancellationToken);
    }

    public async Task<TaskResponse> UpdateAsync(Guid userId, Guid id, TaskUpdateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateTitle(request.Title);
        await EnsureOwnedCategoryAsync(userId, request.CategoryId, cancellationToken);

        var task = await _context.TaskItems.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Task not found");
        }

        var wasCompleted = task.IsCompleted;
        task.Title = request.Title.Trim();
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.SortOrder = request.SortOrder;
        task.IsArchived = request.IsArchived;
        task.CategoryId = request.CategoryId;
        task.UpdatedAt = DateTime.UtcNow;
        task.CompletedAt = !wasCompleted && request.IsCompleted ? DateTime.UtcNow : request.IsCompleted ? task.CompletedAt : null;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(userId, task.Id, cancellationToken);
    }

    public async Task DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _context.TaskItems.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Task not found");
        }

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureOwnedCategoryAsync(Guid userId, Guid? categoryId, CancellationToken cancellationToken)
    {
        if (!categoryId.HasValue)
        {
            return;
        }

        var exists = await _context.Categories.AnyAsync(item => item.Id == categoryId && item.UserId == userId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Category not found");
        }
    }

    private static TaskResponse Map(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        IsCompleted = task.IsCompleted,
        Priority = task.Priority,
        DueDate = task.DueDate,
        SortOrder = task.SortOrder,
        IsArchived = task.IsArchived,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt,
        CompletedAt = task.CompletedAt,
        CategoryId = task.CategoryId,
        CategoryName = task.Category?.Name
    };

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Validation failed");
        }
    }

    private static int NormalizePage(int page) => page < 1 ? 1 : page;

    private static int NormalizePageSize(int pageSize) => pageSize is < 1 or > 100 ? 10 : pageSize;
}
