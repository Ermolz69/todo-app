namespace Todo.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public string? Icon { get; set; }

    public int SortOrder { get; set; }

    public bool IsArchived { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
