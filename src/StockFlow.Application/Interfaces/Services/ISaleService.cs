using StockFlow.Application.DTOs.Sales;

namespace StockFlow.Application.Interfaces.Services;

public interface ISaleService
{
    Task<List<SaleResponseDto>> GetAllAsync();

    Task<SaleResponseDto?> GetByIdAsync(int id);

    Task<SaleResponseDto> CreateAsync(
        CreateSaleDto dto,
        int userId);
}