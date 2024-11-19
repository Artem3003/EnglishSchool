using System;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Data;
using demo_english_school.Models;
using demo_english_school.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebAPI.Services.Tests;

public class TeacherRepositoryTests
{
    private readonly DemoEnglishSchoolContext _context;
    private readonly TeacherRepository _teacherRepository;

    public TeacherRepositoryTests()
    {
        // Use in-memory database for tests
        var options = new DbContextOptionsBuilder<DemoEnglishSchoolContext>()
            .UseInMemoryDatabase("DemoEnglishSchool")
            .Options;

        _context = new DemoEnglishSchoolContext(options);
        _teacherRepository = new TeacherRepository(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Teacher_And_SaveChanges()
    {
        // Arrange
        var user = new User { Id = 1, Username = "Bober", Password = "123", Email = "Bob.gmail.com", FullName = "Bob Smith"};
        await _context.Users.AddAsync(user);
        var teacher = new Teacher 
        { 
            Id = 1, 
            Bio = "Experienced Teacher", 
            Qualification = "MSc", 
            YearsOfExperience = 5, 
            Phone = "123456789", 
            Address = "123 Main St", 
            UserId = 1 
        };

        // Act
        await _teacherRepository.AddAsync(teacher);

        // Assert
        var addedTeacher = await _context.Teachers.FindAsync(1);
        Assert.NotNull(addedTeacher);
        Assert.Equal("Experienced Teacher", addedTeacher.Bio);
        Assert.Equal("123456789", addedTeacher.Phone);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Teacher_If_Found()
    {
        // Arrange
        var user = new User { Id = 1, Username = "Bober", Password = "123", Email = "Bob.gmail.com", FullName = "Bob Smith"};
        await _context.Users.AddAsync(user);
        var teacher = new Teacher { Id = 1, Bio = "Teacher", Qualification = "BSc", UserId = 1 };
        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        // Act
        await _teacherRepository.DeleteAsync(1);

        // Assert
        var deletedTeacher = await _context.Teachers.FindAsync(1);
        Assert.Null(deletedTeacher);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_If_Teacher_Not_Found()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _teacherRepository.DeleteAsync(1));
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Teachers()
    {
        // Arrange
        await _context.Teachers.AddRangeAsync(
            new Teacher { Id = 1, Bio = "Teacher 1", Qualification = "BSc", UserId = 2 },
            new Teacher { Id = 2, Bio = "Teacher 2", Qualification = "MSc", UserId = 3 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _teacherRepository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Teacher_If_Found()
    {
        // Arrange
        var teacher = new Teacher { Id = 1, Bio = "Teacher", Qualification = "BSc", UserId = 2 };
        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        // Act
        var result = await _teacherRepository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Teacher", result.Bio);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_If_Teacher_Not_Found()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _teacherRepository.GetByIdAsync(1));
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Teacher_If_Found()
    {
        // Arrange
        var teacher = new Teacher 
        { 
            Id = 1, 
            Bio = "Old Bio", 
            Qualification = "BSc", 
            YearsOfExperience = 3, 
            Phone = "123456789", 
            Address = "Old Address", 
            UserId = 2 
        };
        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        var updatedTeacher = new Teacher 
        { 
            Id = 1, 
            Bio = "New Bio", 
            Qualification = "MSc", 
            YearsOfExperience = 5, 
            Phone = "987654321", 
            Address = "New Address", 
            UserId = 3 
        };

        // Act
        await _teacherRepository.UpdateAsync(updatedTeacher);

        // Assert
        var teacherFromDb = await _context.Teachers.FindAsync(1);
        Assert.NotNull(teacherFromDb);
        Assert.Equal("New Bio", teacherFromDb.Bio);
        Assert.Equal("New Address", teacherFromDb.Address);
        Assert.Equal(5, teacherFromDb.YearsOfExperience);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_If_Teacher_Not_Found()
    {
        // Arrange
        var updatedTeacher = new Teacher { Id = 1, Bio = "New Bio", Qualification = "MSc", UserId = 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _teacherRepository.UpdateAsync(updatedTeacher));
    }

    [Fact]
    public async Task TeacherExistsAsync_Should_Return_True_If_Teacher_Exists()
    {
        // Arrange
        var teacher = new Teacher { Id = 1, Bio = "Teacher", Qualification = "BSc", UserId = 2 };
        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        // Act
        var result = await _teacherRepository.TeacherExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TeacherExistsAsync_Should_Return_False_If_Teacher_Does_Not_Exist()
    {
        // Act
        var result = await _teacherRepository.TeacherExistsAsync(1);

        // Assert
        Assert.False(result);
    }
}