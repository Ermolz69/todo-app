namespace Todo.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}
