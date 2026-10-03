using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Services;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Persistence.Seed;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        if (!await _context.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role { Name = "Admin" },
                new Role { Name = "Manager" },
                new Role { Name = "Seller" }
            };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
        }

        var adminRole = await _context.Roles
            .FirstAsync(x => x.Name == "Admin");

        var adminExists = await _context.Users
            .AnyAsync(x => x.Email == "admin@stockflow.com");

        if (!adminExists)
        {
            var admin = new User
            {
                Name = "StockFlow Admin",
                Email = "admin@stockflow.com",
                PasswordHash = _passwordHasher.Hash("Admin123*"),
                RoleId = adminRole.Id,
                IsActive = true
            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        var managerRole = await _context.Roles
            .FirstAsync(x => x.Name == "Manager");

        var managerExists = await _context.Users
            .AnyAsync(x => x.Email == "manager@stockflow.com");

        if (!managerExists)
        {
            var manager = new User
            {
                Name = "StockFlow Manager",
                Email = "manager@stockflow.com",
                PasswordHash = _passwordHasher.Hash("Manager123*"),
                RoleId = managerRole.Id,
                IsActive = true
            };

            await _context.Users.AddAsync(manager);
            await _context.SaveChangesAsync();
        }

        var sellerRole = await _context.Roles
            .FirstAsync(x => x.Name == "Seller");

        var sellerExists = await _context.Users
            .AnyAsync(x => x.Email == "seller@stockflow.com");

        if (!sellerExists)
        {
            var seller = new User
            {
                Name = "StockFlow Seller",
                Email = "seller@stockflow.com",
                PasswordHash = _passwordHasher.Hash("Seller123*"),
                RoleId = sellerRole.Id,
                IsActive = true
            };

            await _context.Users.AddAsync(seller);
            await _context.SaveChangesAsync();
        }

        var categoryExists = await _context.Categories
            .AnyAsync(x => x.Name == "Electronics");

        if (!categoryExists)
        {
            var category = new Category
            {
                Name = "Electronics",
                Description = "Default electronics category",
                IsActive = true
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
    }
}