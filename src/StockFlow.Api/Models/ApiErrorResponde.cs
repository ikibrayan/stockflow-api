namespace StockFlow.Api.Models;

public class ApiErrorResponse
{
    public int Status { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? TraceId { get; set; }
}