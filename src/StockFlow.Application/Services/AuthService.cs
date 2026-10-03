using StockFlow.Application.DTOs.Auth;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Interfaces.Services;

namespace StockFlow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var validPassword =
            _passwordHasher.Verify(dto.Password, user.PasswordHash);

        if (!validPassword)
            throw new UnauthorizedAccessException("Invalid credentials.");

        return new AuthResponseDto
        {
            Token = _jwtService.GenerateToken(user),
            ExpiresAt = _jwtService.GetExpiration(),
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.Name
        };
    }
}