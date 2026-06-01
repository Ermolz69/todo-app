using Todo.Domain.Entities;

namespace Todo.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();
}
