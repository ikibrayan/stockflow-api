using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.DTOs.Sales;
using StockFlow.Application.Interfaces.Services;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
	private readonly ISaleService _saleService;

	public SalesController(ISaleService saleService)
	{
		_saleService = saleService;
	}

	[HttpGet]
	[Authorize(Roles = "Admin,Manager")]
	public async Task<IActionResult> GetAll()
	{
		var sales = await _saleService.GetAllAsync();

		return Ok(sales);
	}

	[HttpGet("{id:int}")]
	[Authorize(Roles = "Admin,Manager,Seller")]
	public async Task<IActionResult> GetById(int id)
	{
		var sale = await _saleService.GetByIdAsync(id);

		if (sale is null)
		{
			return NotFound(new
			{
				message = "Sale not found."
			});
		}

		return Ok(sale);
	}

	[HttpPost]
	[Authorize(Roles = "Admin,Manager,Seller")]
	public async Task<IActionResult> Create(
		CreateSaleDto dto)
	{
		var userIdClaim = User.FindFirstValue(
			ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userIdClaim))
		{
			return Unauthorized(new
			{
				status = 401,
				message = "The authenticated user could not be identified."
			});
		}

		if (!int.TryParse(userIdClaim, out var userId))
		{
			return Unauthorized(new
			{
				status = 401,
				message = "The authenticated user is invalid."
			});
		}

		var sale = await _saleService.CreateAsync(
			dto,
			userId);

		return CreatedAtAction(
			nameof(GetById),
			new { id = sale.Id },
			sale);
	}
}