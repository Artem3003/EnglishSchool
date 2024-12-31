using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using demo_english_school.Data;
using demo_english_school.Models;
using demo_english_school.Interfaces;
using WebAPI.Interfaces;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using AutoMapper;
using demo_english_school.Dtos;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using demo_english_school.Options;
using Microsoft.Extensions.Options;

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private const string StudentsCacheKey = "StudentsList";
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IValidator<StudentCreateDto> validator;
        private readonly ILogger<StudentController> logger;
        private readonly IMemoryCache cache;
        private readonly CacheSettings cacheSettings;
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public StudentController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<StudentCreateDto> validator, ILogger<StudentController> logger, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.validator = validator;
            this.logger = logger;
            this.cache = cache;
            this.cacheSettings = cacheSettings.Value;
        }

        // GET: api/student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            if (cache.TryGetValue(StudentsCacheKey, out IEnumerable<StudentDto>? studentsDto))
            {
                logger.LogInformation("Students found in cache.");

            }
            else
            {
                try
                {
                    await semaphoreSlim.WaitAsync();
                    if (cache.TryGetValue(StudentsCacheKey, out studentsDto))
                    {
                        logger.LogInformation("Students found in cache.");
                    }
                    else
                    {
                        logger.LogInformation("Students not found in cache. Fetching from database.");
                        var students = await unitOfWork.StudentRepository.GetAllAsync();
                        studentsDto = this.mapper.Map<IEnumerable<StudentDto>>(students);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(cacheSettings.CacheDuration))
                            .SetAbsoluteExpiration(TimeSpan.FromHours(cacheSettings.CacheDuration))
                            .SetPriority(CacheItemPriority.Normal)
                            .SetSize(1);

                        cache.Set(StudentsCacheKey, studentsDto, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            return Ok(studentsDto);
        }

        // GET: api/student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await unitOfWork.StudentRepository.GetByIdAsync(id);
            if (student == null)
            {
                logger.LogWarning("Student not found");
                return NotFound();
            }
            var studentDto = this.mapper.Map<StudentDto>(student);

            logger.LogInformation("Get student by id");
            return Ok(studentDto);
        }

        // PUT: api/student/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, StudentUpdateDto student)
        {
            var studentDto = this.mapper.Map<Student>(student);
            if (id != studentDto.Id)
            {
                logger.LogWarning("Id not match");
                return BadRequest();
            }

            await unitOfWork.StudentRepository.UpdateAsync(studentDto);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Update student");
            return NoContent();
        }

        // POST: api/student
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentCreateDto>> PostStudent(StudentCreateDto student)
        {
            var validationResult = this.validator.Validate(student);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation error");
                return BadRequest(validationResult.Errors);
            }

            var studentDto = this.mapper.Map<Student>(student);
            await unitOfWork.StudentRepository.AddAsync(studentDto);
            await unitOfWork.SaveAsync();
            cache.Remove(StudentsCacheKey);

            logger.LogInformation("Create student");
            return CreatedAtAction("GetStudent", new { id = studentDto.Id }, studentDto);
        }

        // DELETE: api/student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await unitOfWork.StudentRepository.DeleteAsync(id);
                await unitOfWork.SaveAsync();
                cache.Remove(StudentsCacheKey);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogWarning(ex.Message);
                return NotFound();
            }

            logger.LogInformation("Delete student");
            return NoContent();
        }
    }
}
