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
    public class TeacherControllerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<TeacherCreateDto>> _mockValidator;
        private readonly Mock<ILogger<TeacherController>> _mockLogger;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly IOptions<CacheSettings> _cacheSettings;
        private readonly TeacherController _controller;

        public TeacherControllerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockValidator = new Mock<IValidator<TeacherCreateDto>>();
            _mockLogger = new Mock<ILogger<TeacherController>>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _cacheSettings = Options.Create(new CacheSettings { CacheDuration = 60 });

            _controller = new TeacherController(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockValidator.Object,
                _mockLogger.Object,
                _mockMemoryCache.Object,
                _cacheSettings
            );
        }

        [Fact]
        public async Task GetTeachers_Should_Return_All_Teachers()
        {
            // Arrange
            var teachers = new List<Teacher>
            {
                new Teacher { Id = 1, Bio = "Bio1", UserId = 1 },
                new Teacher { Id = 2, Bio = "Bio2", UserId = 2 }
            };
            var teachersDto = new List<TeacherDto>
            {
                new TeacherDto { Id = 1, Bio = "Bio1", UserId = 1 },
                new TeacherDto { Id = 2, Bio = "Bio2", UserId = 2 }
            };

            _mockUnitOfWork.Setup(u => u.TeacherRepository.GetAllAsync()).ReturnsAsync(teachers);
            _mockMapper.Setup(m => m.Map<IEnumerable<TeacherDto>>(teachers)).Returns(teachersDto);

            object? cacheEntry = null;
            _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheEntry)).Returns(false);
            _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _controller.GetTeachers();

            // Assert
            Assert.NotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<IEnumerable<TeacherDto>>(okResult.Value);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetTeacher_Should_Return_Teacher_If_Found()
        {
            // Arrange
            var teacher = new Teacher { Id = 1, Bio = "Bio1", UserId = 1 };
            var teacherDto = new TeacherDto { Id = 1, Bio = "Bio1", UserId = 1 };

            _mockUnitOfWork.Setup(u => u.TeacherRepository.GetByIdAsync(1)).ReturnsAsync(teacher);
            _mockMapper.Setup(m => m.Map<TeacherDto>(teacher)).Returns(teacherDto);

            // Act
            var result = await _controller.GetTeacher(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<TeacherDto>(okResult.Value);
            Assert.NotNull(okResult.Value);
            Assert.Equal(teacherDto.Id, ((TeacherDto)okResult.Value).Id);
        }

        [Fact]
        public async Task GetTeacher_Should_Return_NotFound_If_Teacher_Not_Found()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.TeacherRepository.GetByIdAsync(1)).ReturnsAsync((Teacher)null);

            // Act
            var result = await _controller.GetTeacher(1);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task PostTeacher_Should_Add_Teacher_And_SaveChanges()
        {
            // Arrange
            var teacherCreateDto = new TeacherCreateDto { Id = 1, Bio = "Bio1", UserId = 1 };
            var teacher = new Teacher { Id = 1, Bio = "Bio1", UserId = 1 };
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.Setup(v => v.Validate(teacherCreateDto)).Returns(validationResult);
            _mockMapper.Setup(m => m.Map<Teacher>(teacherCreateDto)).Returns(teacher);
            _mockMemoryCache.Setup(m => m.Remove(It.IsAny<object>()));
            _mockUnitOfWork.Setup(u => u.TeacherRepository.AddAsync(teacher)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostTeacher(teacherCreateDto);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.IsAssignableFrom<Teacher>(createdAtActionResult.Value);
            _mockUnitOfWork.Verify(u => u.TeacherRepository.AddAsync(teacher), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task PutTeacher_Should_Update_Teacher_If_Id_Matches()
        {
            // Arrange
            var teacherUpdateDto = new TeacherUpdateDto { Bio = "Bio1" };
            var teacher = new Teacher { Id = 1, Bio = "Bio1", UserId = 1 };

            _mockMapper.Setup(m => m.Map<Teacher>(teacherUpdateDto)).Returns(teacher);
            _mockUnitOfWork.Setup(u => u.TeacherRepository.GetAllAsync()).ReturnsAsync(new List<Teacher> { teacher });
            _mockUnitOfWork.Setup(u => u.TeacherRepository.UpdateAsync(teacher)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutTeacher(1, teacherUpdateDto);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.NotNull(noContentResult);
            _mockUnitOfWork.Verify(u => u.TeacherRepository.UpdateAsync(teacher), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task PutTeacher_Should_Return_BadRequest_If_Id_Not_Match()
        {
            // Arrange
            var teacherUpdateDto = new TeacherUpdateDto { Bio = "Bio1" };
            var teacher = new Teacher { Id = 1, Bio = "Bio1", UserId = 1 };

            _mockMapper.Setup(m => m.Map<Teacher>(teacherUpdateDto)).Returns(teacher);
            _mockUnitOfWork.Setup(u => u.TeacherRepository.GetAllAsync()).ReturnsAsync(new List<Teacher> { teacher });

            // Act
            var result = await _controller.PutTeacher(2, teacherUpdateDto);

            // Assert
            var badRequestResult = result as BadRequestResult;
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async Task DeleteTeacher_Should_Delete_Teacher_If_Found()
        {
            // Arrange
            var teacher = new Teacher { Id = 1, Bio = "Bio1", UserId = 1 };

            _mockUnitOfWork.Setup(u => u.TeacherRepository.GetByIdAsync(1)).ReturnsAsync(teacher);

            // Act
            var result = await _controller.DeleteTeacher(1);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.NotNull(noContentResult);
            _mockUnitOfWork.Verify(u => u.TeacherRepository.DeleteAsync(1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTeacher_Should_Return_NotFound_If_Teacher_Not_Found()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.TeacherRepository.GetByIdAsync(1)).ReturnsAsync((Teacher?)null);
            _mockUnitOfWork.Setup(u => u.TeacherRepository.DeleteAsync(1)).Throws(new ArgumentNullException("Teacher not found"));

            // Act
            var result = await _controller.DeleteTeacher(1);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }
    }
}