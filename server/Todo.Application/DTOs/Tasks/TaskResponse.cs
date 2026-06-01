namespace Todo.Application.DTOs.Tasks;

public class TaskResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public int Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public int SortOrder { get; set; }

    public bool IsArchived { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid? CategoryId { get; set; }

    public string? CategoryName { get; set; }
}
