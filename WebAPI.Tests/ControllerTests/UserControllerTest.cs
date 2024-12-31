using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using demo_english_school.Controllers;
using demo_english_school.Dtos;
using demo_english_school.Interfaces;
using demo_english_school.Models;
using demo_english_school.Options;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WebAPI.Interfaces;
using Xunit;

namespace WebAPI.Tests.ControllerTests
{
    public class UserControllerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<UserCreateDto>> _mockValidator;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly IOptions<CacheSettings> _cacheSettings;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockValidator = new Mock<IValidator<UserCreateDto>>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _cacheSettings = Options.Create(new CacheSettings { CacheDuration = 60 });

            _controller = new UserController(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockValidator.Object,
                _mockLogger.Object,
                _mockMemoryCache.Object,
                _cacheSettings
            );
        }

        [Fact]
        public async Task GetUsers_Should_Return_All_Users()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com" },
                new User { Id = 2, Username = "user2", Email = "user2@example.com" }
            };
            var usersDto = new List<UserDto>
            {
                new UserDto { Id = 1, Username = "user1", Email = "user1@example.com" },
                new UserDto { Id = 2, Username = "user2", Email = "user2@example.com" }
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetAllAsync()).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(usersDto);

            object? cacheEntry = null;
            _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheEntry)).Returns(false);
            _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _controller.GetUsers();

            // Assert
            Assert.NotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetUser_Should_Return_User_If_Found()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1", Email = "user1@example.com" };
            var userDto = new UserDto { Id = 1, Username = "user1", Email = "user1@example.com" };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _controller.GetUser(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<UserDto>(okResult.Value);
            Assert.NotNull(okResult.Value);
            Assert.Equal(userDto.Id, ((UserDto)okResult.Value).Id);
        }

        [Fact]
        public async Task GetUser_Should_Return_NotFound_If_User_Not_Found()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1)).ReturnsAsync((User?)null);

            // Act
            var result = await _controller.GetUser(1);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task PostUser_Should_Add_User_And_SaveChanges()
        {
            // Arrange
            var userCreateDto = new UserCreateDto { Id = 1, Username = "user1", Email = "user1@example.com" };
            var user = new User { Id = 1, Username = "user1", Email = "user1@example.com" };
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.Setup(v => v.Validate(userCreateDto)).Returns(validationResult);
            _mockMapper.Setup(m => m.Map<User>(userCreateDto)).Returns(user);
            _mockMemoryCache.Setup(m => m.Remove(It.IsAny<object>()));
            _mockUnitOfWork.Setup(u => u.UserRepository.AddAsync(user)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostUser(userCreateDto);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.IsAssignableFrom<User>(createdAtActionResult.Value);
            _mockUnitOfWork.Verify(u => u.UserRepository.AddAsync(user), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task PutUser_Should_Update_User_If_Id_Matches()
        {
            // Arrange
            var userUpdateDto = new UserUpdateDto { Username = "user1", Email = "user1@example.com" };
            var user = new User { Id = 1, Username = "user1", Email = "user1@example.com" };

            _mockMapper.Setup(m => m.Map<User>(userUpdateDto)).Returns(user);
            _mockUnitOfWork.Setup(u => u.UserRepository.UpdateAsync(user)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutUser(1, userUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUnitOfWork.Verify(u => u.UserRepository.UpdateAsync(user), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task PutUser_Should_Return_BadRequest_If_Id_Not_Match()
        {
            // Arrange
            var userUpdateDto = new UserUpdateDto { Username = "user1", Email = "user1@example.com" };
            var user = new User { Id = 1, Username = "user1", Email = "user1@example.com" };

            _mockMapper.Setup(m => m.Map<User>(userUpdateDto)).Returns(user);

            // Act
            var result = await _controller.PutUser(2, userUpdateDto);

            // Assert
            var badRequestResult = result as BadRequestResult;
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async Task DeleteUser_Should_Delete_User_If_Found()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1", Email = "user1@example.com" };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.NotNull(noContentResult);
            _mockUnitOfWork.Verify(u => u.UserRepository.DeleteAsync(1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_Should_Return_NotFound_If_User_Not_Found()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1)).ReturnsAsync((User)null);
            _mockUnitOfWork.Setup(u => u.UserRepository.DeleteAsync(1)).Throws(new ArgumentNullException("User not found"));

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }
    }
}