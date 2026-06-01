using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.DTOs.Categories;
using Todo.Application.Exceptions;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<CategoryResponse>> GetAllAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = NormalizePage(page);
        pageSize = NormalizePageSize(pageSize);

        var query = _context.Categories
            .AsNoTracking()
            .Where(category => category.UserId == userId)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(category => Map(category))
            .ToListAsync(cancellationToken);

        return ToPagedResponse(items, page, pageSize, totalCount);
    }

    public async Task<CategoryResponse> CreateAsync(Guid userId, CategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Color = request.Color,
            Icon = request.Icon,
            SortOrder = request.SortOrder,
            UserId = userId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<CategoryResponse> UpdateAsync(Guid userId, Guid id, CategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var category = await GetOwnedCategoryAsync(userId, id, cancellationToken);
        category.Name = request.Name.Trim();
        category.Color = request.Color;
        category.Icon = request.Icon;
        category.SortOrder = request.SortOrder;
        category.IsArchived = request.IsArchived;

        await _context.SaveChangesAsync(cancellationToken);
        return Map(category);
    }

    public async Task DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var category = await GetOwnedCategoryAsync(userId, id, cancellationToken);
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<Category> GetOwnedCategoryAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);
        return category ?? throw new NotFoundException("Category not found");
    }

    private static CategoryResponse Map(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Color = category.Color,
        Icon = category.Icon,
        SortOrder = category.SortOrder,
        IsArchived = category.IsArchived
    };

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Validation failed");
        }
    }

    private static int NormalizePage(int page) => page < 1 ? 1 : page;

    private static int NormalizePageSize(int pageSize) => pageSize is < 1 or > 100 ? 20 : pageSize;

    private static PagedResponse<CategoryResponse> ToPagedResponse(IEnumerable<CategoryResponse> items, int page, int pageSize, int totalCount) => new()
    {
        Items = items,
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    };
}
