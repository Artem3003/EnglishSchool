using demo_english_school.Services;
using demo_english_school.Models;
using demo_english_school.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System.Linq;

namespace  WebAPI.Services.Tests;

public class UserServiceTests
{
    private DemoEnglishSchoolContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DemoEnglishSchoolContext>()
            .UseInMemoryDatabase(databaseName: "DemoEnglishSchoolTest")
            .Options;
        return new DemoEnglishSchoolContext(options);
    }

    private UserService GetUserService(DemoEnglishSchoolContext context)
    {
        return new UserService(context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);
        var user = new User { Username = "testuser", Email = "test@example.com", FullName = "Test User" };

        // Act
        await service.AddAsync(user);
        var users = await context.Users.ToListAsync();

        // Assert
        Assert.Single(users);
        Assert.Equal("testuser", users[0].Username);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);
        var user = new User { Username = "testuser", Email = "test@example.com", FullName = "Test User" };
        await service.AddAsync(user);

        // Act
        await service.DeleteAsync(user.Id);
        var users = await context.Users.ToListAsync();

        // Assert
        Assert.Empty(users);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowIfUserNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.DeleteAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);
        var user1 = new User { Username = "user1", Email = "user1@example.com" };
        var user2 = new User { Username = "user2", Email = "user2@example.com" };
        await service.AddAsync(user1);
        await service.AddAsync(user2);

        // Act
        var users = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, users.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserById()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);
        var user = new User { Username = "testuser", Email = "test@example.com", FullName = "Test User" };
        await service.AddAsync(user);

        // Act
        var result = await service.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowIfUserNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetByIdAsync(999));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);
        var user = new User { Username = "testuser", Email = "test@example.com", FullName = "Test User" };
        await service.AddAsync(user);

        // Act
        user.FullName = "Updated User";
        await service.UpdateAsync(user);

        // Assert
        var updatedUser = await service.GetByIdAsync(user.Id);
        Assert.Equal("Updated User", updatedUser.FullName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowIfUserNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);
        var nonExistentUser = new User { Id = 999, Username = "nonexistent", FullName = "Non Existent" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(nonExistentUser));
    }

    [Fact]
    public async Task UserExistsAsync_ShouldReturnTrueIfUserExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);
        var user = new User { Username = "testuser", Email = "test@example.com", FullName = "Test User" };
        await service.AddAsync(user);

        // Act
        var exists = await service.UserExistsAsync(user.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task UserExistsAsync_ShouldReturnFalseIfUserDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = GetUserService(context);

        // Act
        var exists = await service.UserExistsAsync(999);

        // Assert
        Assert.False(exists);
    }
}