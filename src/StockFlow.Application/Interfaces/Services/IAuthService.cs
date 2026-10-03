using StockFlow.Application.DTOs.Auth;

namespace StockFlow.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}