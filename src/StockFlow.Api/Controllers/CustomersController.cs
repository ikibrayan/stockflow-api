using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.DTOs.Customers;
using StockFlow.Application.Interfaces.Services;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(
        ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Seller")]
    public async Task<IActionResult> GetAll()
    {
        var customers =
            await _customerService.GetAllAsync();

        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Manager,Seller")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer =
            await _customerService.GetByIdAsync(id);

        if (customer is null)
        {
            return NotFound(new
            {
                message = "Customer not found."
            });
        }

        return Ok(customer);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Seller")]
    public async Task<IActionResult> Create(
        CreateCustomerDto dto)
    {
        var customer =
            await _customerService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            customer);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCustomerDto dto)
    {
        var updated =
            await _customerService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Customer not found."
            });
        }

        return NoContent();
    }
}