using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Application.DTOs.Auth;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Services;

public class AuthService : IAuthService
{
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(IApplicationDbContext context, IJwtService jwtService, PasswordHasher<User> passwordHasher)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRegisterRequest(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await _context.Users.AnyAsync(user => user.Email == email, cancellationToken);
        if (exists)
        {
            throw new ValidationException("Validation failed");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = request.UserName.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);

        var response = await IssueTokensAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ValidateLoginRequest(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(item => item.Email == email, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException();
        }

        var response = await IssueTokensAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return response;
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRefreshRequest(request);

        var currentToken = await _context.RefreshTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(token => token.Token == request.RefreshToken, cancellationToken);

        if (currentToken is null || currentToken.IsRevoked || currentToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException();
        }

        currentToken.IsRevoked = true;

        var response = await IssueTokensAsync(currentToken.User, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return response;
    }

    public async Task LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRefreshRequest(request);

        var token = await _context.RefreshTokens.FirstOrDefaultAsync(item => item.Token == request.RefreshToken, cancellationToken);
        if (token is null)
        {
            return;
        }

        token.IsRevoked = true;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private Task<AuthResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime);

        _context.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            ExpiresAt = refreshTokenExpiresAt,
            UserId = user.Id
        });

        return Task.FromResult(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiresAt = DateTime.UtcNow.Add(AccessTokenLifetime),
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        });
    }

    private static void ValidateRegisterRequest(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Password.Length < 6)
        {
            throw new ValidationException("Validation failed");
        }
    }

    private static void ValidateLoginRequest(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Validation failed");
        }
    }

    private static void ValidateRefreshRequest(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new ValidationException("Validation failed");
        }
    }
}
