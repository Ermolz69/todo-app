namespace Todo.Application.DTOs.Categories;

public class CategoryCreateRequest
{
    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public string? Icon { get; set; }

    public int SortOrder { get; set; }
}
