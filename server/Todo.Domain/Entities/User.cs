namespace Todo.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
