namespace Todo.Application.DTOs.Tasks;

public class TaskQuery
{
    public string? Search { get; set; }

    public Guid? CategoryId { get; set; }

    public bool? IsCompleted { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
