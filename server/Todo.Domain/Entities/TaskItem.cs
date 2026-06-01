namespace Todo.Domain.Entities;

public class TaskItem
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

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    public Category? Category { get; set; }
}
