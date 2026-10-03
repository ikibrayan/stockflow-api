using StockFlow.Domain.Entities;

namespace StockFlow.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpiration();
}