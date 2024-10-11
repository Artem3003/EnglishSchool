using System;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Data;
using demo_english_school.Models;
using demo_english_school.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebAPI.Services.Tests;

public class StudentServiceTests
{
    private readonly DemoEnglishSchoolContext _context;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        // Using InMemory database with the same name as configured in the context
        var options = new DbContextOptionsBuilder<DemoEnglishSchoolContext>()
            .UseInMemoryDatabase("DemoEnglishSchool")
            .Options;

        _context = new DemoEnglishSchoolContext(options);
        _studentService = new StudentService(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Student_And_SaveChanges()
    {
        // Arrange
        var user = new User { Id = 1, Username = "Bober", Password = "123", Email = "Bob.gmail.com", FullName = "Bob Smith"};
        await _context.Users.AddAsync(user);
        var student = new Student { Id = 1, Address = "123 Main St", DateOfBirth = DateTime.Now.AddYears(-20), Phone = "1234567890", UserId = 1 };

        // Act
        await _studentService.AddAsync(student);

        // Assert
        var addedStudent = await _context.Students.FindAsync(1);
        Assert.NotNull(addedStudent);
        Assert.Equal("123 Main St", addedStudent.Address);
        Assert.Equal(1, addedStudent.UserId);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Student_If_Found()
    {
        // Arrange
        var user = new User { Id = 1, Username = "Bober", Password = "123", Email = "Bob.gmail.com", FullName = "Bob Smith"};
        await _context.Users.AddAsync(user);
        var student = new Student { Id = 1, Address = "123 Main St", DateOfBirth = DateTime.Now.AddYears(-20), Phone = "1234567890", UserId = 1 };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        await _studentService.DeleteAsync(1);

        // Assert
        var deletedStudent = await _context.Students.FindAsync(1);
        Assert.Null(deletedStudent);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_If_Student_Not_Found()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _studentService.DeleteAsync(1));
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Students()
    {
        // Arrange
        await _context.Students.AddRangeAsync(
            new Student { Id = 1, Address = "123 Main St", DateOfBirth = DateTime.Now.AddYears(-20), Phone = "1234567890", UserId = 2 },
            new Student { Id = 2, Address = "456 Second St", DateOfBirth = DateTime.Now.AddYears(-22), Phone = "9876543210", UserId = 3 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _studentService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Student_If_Found()
    {
        // Arrange
        var student = new Student { Id = 1, Address = "123 Main St", DateOfBirth = DateTime.Now.AddYears(-20), Phone = "1234567890", UserId = 2 };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _studentService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123 Main St", result.Address);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_If_Student_Not_Found()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _studentService.GetByIdAsync(1));
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Student_If_Found()
    {
        // Arrange
        var student = new Student { Id = 1, Address = "Old Address", DateOfBirth = DateTime.Now.AddYears(-20), Phone = "1234567890", UserId = 2 };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        var updatedStudent = new Student { Id = 1, Address = "New Address", DateOfBirth = DateTime.Now.AddYears(-22), Phone = "9876543210", UserId = 3 };

        // Act
        await _studentService.UpdateAsync(updatedStudent);

        // Assert
        var studentFromDb = await _context.Students.FindAsync(1);
        Assert.NotNull(studentFromDb);
        Assert.Equal("New Address", studentFromDb.Address);
        Assert.Equal(3, studentFromDb.UserId);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_If_Student_Not_Found()
    {
        // Arrange
        var updatedStudent = new Student { Id = 1, Address = "New Address", DateOfBirth = DateTime.Now.AddYears(-22), Phone = "9876543210", UserId = 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _studentService.UpdateAsync(updatedStudent));
    }

    [Fact]
    public async Task StudentExistsAsync_Should_Return_True_If_Student_Exists()
    {
        // Arrange
        var student = new Student { Id = 1, Address = "123 Main St", DateOfBirth = DateTime.Now.AddYears(-20), Phone = "1234567890", UserId = 2 };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _studentService.StudentExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task StudentExistsAsync_Should_Return_False_If_Student_Does_Not_Exist()
    {
        // Act
        var result = await _studentService.StudentExistsAsync(1);

        // Assert
        Assert.False(result);
    }
}