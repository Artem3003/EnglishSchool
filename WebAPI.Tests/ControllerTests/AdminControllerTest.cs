using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using demo_english_school.Controllers;
using demo_english_school.Dtos;
using demo_english_school.Options;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WebAPI.Mocks.Tests;
using Xunit;
using WebAPI.Interfaces;
using demo_english_school.Models;

namespace WebAPI.Tests.ControllerTests;

public class AdminControllerTest
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<AdminCreateDto>> _mockValidator;
    private readonly Mock<ILogger<AdminController>> _mockLogger;
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly IOptions<CacheSettings> _cacheSettings;
    private readonly AdminController _controller;

    public AdminControllerTest()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockValidator = new Mock<IValidator<AdminCreateDto>>();
        _mockLogger = new Mock<ILogger<AdminController>>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        _cacheSettings = Options.Create(new CacheSettings { CacheDuration = 60 });

        _controller = new AdminController(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mockMemoryCache.Object,
            _cacheSettings
        );
    }

    [Fact]
    public async Task GetAdmins_Should_Return_All_Admins()
    {
        // Arrange
        var admins = new List<Admin>
            {
                new Admin { Id = 1, Role = "Admin", UserId = 1 },
                new Admin { Id = 2, Role = "Manager", UserId = 2 }
            };
        var adminsDto = new List<AdminDto>
            {
                new AdminDto { Id = 1, Role = "Admin", UserId = 1 },
                new AdminDto { Id = 2, Role = "Manager", UserId = 2 }
            };

        _mockUnitOfWork.Setup(u => u.AdminRepository.GetAllAsync()).ReturnsAsync(admins);
        _mockMapper.Setup(m => m.Map<IEnumerable<AdminDto>>(admins)).Returns(adminsDto);

        object? cacheEntry = null;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheEntry)).Returns(false);
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

        // Act
        var result = await _controller.GetAdmins();

        // Assert
        Assert.NotNull(result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.IsAssignableFrom<IEnumerable<AdminDto>>(okResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAdmin_Should_Return_Admin_If_Found()
    {
        // Arrange
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 1 };
        var adminDto = new AdminDto { Id = 1, Role = "Admin", UserId = 1 };

        _mockUnitOfWork.Setup(u => u.AdminRepository.GetByIdAsync(1)).ReturnsAsync(admin);
        _mockMapper.Setup(m => m.Map<AdminDto>(admin)).Returns(adminDto);

        // Act
        var result = await _controller.GetAdmin(1);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.IsAssignableFrom<AdminDto>(okResult.Value);
        Assert.NotNull(okResult.Value);
        Assert.Equal(adminDto.Id, ((AdminDto)okResult.Value).Id);
    }

    [Fact]
    public async Task GetAdmin_Should_Return_NotFound_If_Admin_Not_Found()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.AdminRepository.GetByIdAsync(1)).ReturnsAsync((Admin?)null);

        // Act
        var result = await _controller.GetAdmin(1);

        // Assert
        var notFoundResult = result.Result as NotFoundResult;
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task PostAdmin_Should_Add_Admin_And_SaveChanges()
    {
        // Arrange
        var adminCreateDto = new AdminCreateDto { Id = 1, Role = "Admin", UserId = 1 };
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 1 };
        var validationResult = new FluentValidation.Results.ValidationResult();

        _mockValidator.Setup(v => v.Validate(adminCreateDto)).Returns(validationResult);
        _mockMapper.Setup(m => m.Map<Admin>(adminCreateDto)).Returns(admin);
        _mockMemoryCache.Setup(m => m.Remove(It.IsAny<object>()));
        _mockUnitOfWork.Setup(u => u.AdminRepository.AddAsync(admin)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PostAdmin(adminCreateDto);

        // Assert
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdAtActionResult);
        Assert.IsAssignableFrom<Admin>(createdAtActionResult.Value);
        _mockUnitOfWork.Verify(u => u.AdminRepository.AddAsync(admin), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
    }

     [Fact]
    public async Task PutAdmin_Should_Update_Admin_If_Id_Matches()
    {
        // Arrange
        var adminUpdateDto = new AdminUpdateDto { Id = 1, Role = "Admin" };
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 1 };

        _mockMapper.Setup(m => m.Map<Admin>(adminUpdateDto)).Returns(admin);
        _mockUnitOfWork.Setup(u => u.AdminRepository.UpdateAsync(admin)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PutAdmin(1, adminUpdateDto);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        _mockUnitOfWork.Verify(u => u.AdminRepository.UpdateAsync(admin), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task PutAdmin_Should_Return_BadRequest_If_Id_Not_Match()
    {
        // Arrange
        var adminUpdateDto = new AdminUpdateDto { Id = 1, Role = "Admin", User = new UserUpdateDto { Username = "admin" } };
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 1, User = new User { Id = 1, Username = "admin" } };

        _mockMapper.Setup(m => m.Map<Admin>(adminUpdateDto)).Returns(admin);

        // Act
        var result = await _controller.PutAdmin(2, adminUpdateDto);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.NotNull(badRequestResult);
    }

    [Fact]
    public async Task DeleteAdmin_Should_Delete_Admin_If_Found()
    {
        // Arrange
        var admin = new Admin { Id = 1, Role = "Admin", UserId = 1 };

        _mockUnitOfWork.Setup(u => u.AdminRepository.GetByIdAsync(1)).ReturnsAsync(admin);

        // Act
        var result = await _controller.DeleteAdmin(1);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        _mockUnitOfWork.Verify(u => u.AdminRepository.DeleteAsync(1), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAdmin_Should_Return_NotFound_If_Admin_Not_Found()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.AdminRepository.GetByIdAsync(1)).ReturnsAsync((Admin?)null);
        _mockUnitOfWork.Setup(u => u.AdminRepository.DeleteAsync(1)).Throws(new ArgumentNullException("Admin not found"));

        // Act
        var result = await _controller.DeleteAdmin(1);

        // Assert
        var notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
    }
}