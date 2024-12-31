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
    public class StudentControllerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<StudentCreateDto>> _mockValidator;
        private readonly Mock<ILogger<StudentController>> _mockLogger;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly IOptions<CacheSettings> _cacheSettings;
        private readonly StudentController _controller;

        public StudentControllerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockValidator = new Mock<IValidator<StudentCreateDto>>();
            _mockLogger = new Mock<ILogger<StudentController>>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _cacheSettings = Options.Create(new CacheSettings { CacheDuration = 60 });

            _controller = new StudentController(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockValidator.Object,
                _mockLogger.Object,
                _mockMemoryCache.Object,
                _cacheSettings
            );
        }

        [Fact]
        public async Task GetStudents_Should_Return_All_Students()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 },
                new Student { Id = 2, DateOfBirth = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 2 }
            };
            var studentsDto = new List<StudentDto>
            {
                new StudentDto { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 },
                new StudentDto { Id = 2, DateOfBirth = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 2 }
            };

            _mockUnitOfWork.Setup(u => u.StudentRepository.GetAllAsync()).ReturnsAsync(students);
            _mockMapper.Setup(m => m.Map<IEnumerable<StudentDto>>(students)).Returns(studentsDto);

            object? cacheEntry = null;
            _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheEntry)).Returns(false);
            _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _controller.GetStudents();

            // Assert
            Assert.NotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<IEnumerable<StudentDto>>(okResult.Value);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetStudent_Should_Return_Student_If_Found()
        {
            // Arrange
            var student = new Student { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 };
            var studentDto = new StudentDto { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 };

            _mockUnitOfWork.Setup(u => u.StudentRepository.GetByIdAsync(1)).ReturnsAsync(student);
            _mockMapper.Setup(m => m.Map<StudentDto>(student)).Returns(studentDto);

            // Act
            var result = await _controller.GetStudent(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<StudentDto>(okResult.Value);
            Assert.NotNull(okResult.Value);
            Assert.Equal(studentDto.Id, ((StudentDto)okResult.Value).Id);
        }

        [Fact]
        public async Task GetStudent_Should_Return_NotFound_If_Student_Not_Found()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.StudentRepository.GetByIdAsync(1)).ReturnsAsync((Student?)null);

            // Act
            var result = await _controller.GetStudent(1);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task PostStudent_Should_Add_Student_And_SaveChanges()
        {
            // Arrange
            var studentCreateDto = new StudentCreateDto { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 };
            var student = new Student { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 };
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.Setup(v => v.Validate(studentCreateDto)).Returns(validationResult);
            _mockMapper.Setup(m => m.Map<Student>(studentCreateDto)).Returns(student);
            _mockMemoryCache.Setup(m => m.Remove(It.IsAny<object>()));
            _mockUnitOfWork.Setup(u => u.StudentRepository.AddAsync(student)).Returns(Task.FromResult(student));

            // Act
            var result = await _controller.PostStudent(studentCreateDto);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.IsAssignableFrom<Student>(createdAtActionResult.Value);
            _mockUnitOfWork.Verify(u => u.StudentRepository.AddAsync(student), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task PutStudent_Should_Update_Student_If_Id_Matches()
        {
            // Arrange
            var studentUpdateDto = new StudentUpdateDto { DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            var student = new Student { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 };

            _mockMapper.Setup(m => m.Map<Student>(studentUpdateDto)).Returns(student);
            _mockUnitOfWork.Setup(u => u.StudentRepository.UpdateAsync(student)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutStudent(1, studentUpdateDto);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.NotNull(noContentResult);
            _mockUnitOfWork.Verify(u => u.StudentRepository.UpdateAsync(student), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task PutStudent_Should_Return_BadRequest_If_Id_Not_Match()
        {
            // Arrange
            var studentUpdateDto = new StudentUpdateDto { DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            var student = new Student { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 };

            _mockMapper.Setup(m => m.Map<Student>(studentUpdateDto)).Returns(student);

            // Act
            var result = await _controller.PutStudent(2, studentUpdateDto);

            // Assert
            var badRequestResult = result as BadRequestResult;
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async Task DeleteStudent_Should_Delete_Student_If_Found()
        {
            // Arrange
            var student = new Student { Id = 1, DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), UserId = 1 };

            _mockUnitOfWork.Setup(u => u.StudentRepository.GetByIdAsync(1)).ReturnsAsync(student);

            // Act
            var result = await _controller.DeleteStudent(1);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.NotNull(noContentResult);
            _mockUnitOfWork.Verify(u => u.StudentRepository.DeleteAsync(1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteStudent_Should_Return_NotFound_If_Student_Not_Found()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.StudentRepository.GetByIdAsync(1)).ReturnsAsync((Student?)null);
            _mockUnitOfWork.Setup(u => u.StudentRepository.DeleteAsync(1)).Throws(new ArgumentNullException("Student not found"));

            // Act
            var result = await _controller.DeleteStudent(1);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }
    }
}