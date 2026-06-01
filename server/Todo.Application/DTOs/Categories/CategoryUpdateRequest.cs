namespace Todo.Application.DTOs.Categories;

public class CategoryUpdateRequest
{
    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public string? Icon { get; set; }

    public int SortOrder { get; set; }

    public bool IsArchived { get; set; }
}
