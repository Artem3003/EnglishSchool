using System;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Data;
using demo_english_school.Models;
using demo_english_school.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebAPI.Services.Tests;

public class AdminServiceTests
{
    private readonly DemoEnglishSchoolContext _context;
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        // Using InMemory database with the same name as configured in the context
        var options = new DbContextOptionsBuilder<DemoEnglishSchoolContext>()
            .UseInMemoryDatabase("DemoEnglishSchool")
            .Options;

        // Create a new context instance for each test
        _context = new DemoEnglishSchoolContext(options);
        _adminService = new AdminService(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Admin_And_SaveChanges()
    {
        // Arrange
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 2 };

        // Act
        await _adminService.AddAsync(admin);

        // Assert
        var addedAdmin = await _context.Admins.FindAsync(1);
        Assert.NotNull(addedAdmin);
        Assert.Equal("Admin", addedAdmin.Role);
        Assert.Equal(2, addedAdmin.UserId);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Admin_If_Found()
    {
        // Arrange
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 2 };
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        // Act
        await _adminService.DeleteAsync(1);

        // Assert
        var deletedAdmin = await _context.Admins.FindAsync(1);
        Assert.Null(deletedAdmin);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_If_Admin_Not_Found()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _adminService.DeleteAsync(1));
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Admins()
    {
        // Arrange
        await _context.Admins.AddRangeAsync(
            new Admin { Id = 1, Role = "Admin", UserId = 2 },
            new Admin { Id = 2, Role = "Manager", UserId = 3 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _adminService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Admin_If_Found()
    {
        // Arrange
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 2 };
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        // Act
        var result = await _adminService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_If_Admin_Not_Found()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _adminService.GetByIdAsync(1));
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Admin_If_Found()
    {
        // Arrange
        var admin = new Admin { Id = 1, Role = "OldRole", UserId = 2 };
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        var updatedAdmin = new Admin { Id = 1, Role = "NewRole", UserId = 3 };

        // Act
        await _adminService.UpdateAsync(updatedAdmin);

        // Assert
        var adminFromDb = await _context.Admins.FindAsync(1);
        Assert.NotNull(adminFromDb);
        Assert.Equal("NewRole", adminFromDb.Role);
        Assert.Equal(3, adminFromDb.UserId);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_If_Admin_Not_Found()
    {
        // Arrange
        var updatedAdmin = new Admin { Id = 1, Role = "NewRole", UserId = 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _adminService.UpdateAsync(updatedAdmin));
    }

    [Fact]
    public async Task AdminExistsAsync_Should_Return_True_If_Admin_Exists()
    {
        // Arrange
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 2 };
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        // Act
        var result = await _adminService.AdminExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AdminExistsAsync_Should_Return_False_If_Admin_Does_Not_Exist()
    {
        // Act
        var result = await _adminService.AdminExistsAsync(1);

        // Assert
        Assert.False(result);
    }
}