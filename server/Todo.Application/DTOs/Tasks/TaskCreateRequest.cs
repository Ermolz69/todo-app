namespace Todo.Application.DTOs.Tasks;

public class TaskCreateRequest
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public int SortOrder { get; set; }

    public Guid? CategoryId { get; set; }
}
