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
using AutoMapper;
using demo_english_school.Dtos;
using demo_english_school.Validator;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using demo_english_school.Options;
using Microsoft.Extensions.Options;

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private const string TeachersCacheKey = "TeachersList";
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IValidator<TeacherCreateDto> validator;
        private readonly ILogger<TeacherController> logger;
        private readonly IMemoryCache cache;
        private readonly CacheSettings cacheSettings;
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public TeacherController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<TeacherCreateDto> validator, ILogger<TeacherController> logger, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.validator = validator;
            this.logger = logger;
            this.cache = cache;
            this.cacheSettings = cacheSettings.Value;
        }

        // GET: api/teacher
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachers()
        {
            if (cache.TryGetValue(TeachersCacheKey, out IEnumerable<TeacherDto>? teachersDto))
            {
                logger.LogInformation("Teachers found in cache.");
                return Ok(teachersDto);
            }
            else
            {
                try
                {
                    await semaphoreSlim.WaitAsync();
                    if (cache.TryGetValue(TeachersCacheKey, out teachersDto))
                    {
                        logger.LogInformation("Teachers found in cache.");
                    }
                    else
                    {
                        logger.LogInformation("Teachers not found in cache. Fetching from database.");
                        var teachers = await unitOfWork.TeacherRepository.GetAllAsync();
                        teachersDto = this.mapper.Map<IEnumerable<TeacherDto>>(teachers);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(cacheSettings.CacheDuration))
                            .SetAbsoluteExpiration(TimeSpan.FromHours(cacheSettings.CacheDuration))
                            .SetPriority(CacheItemPriority.Normal)
                            .SetSize(1);

                        cache.Set(TeachersCacheKey, teachersDto, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            return Ok(teachersDto);
        }

        // GET: api/teacher/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacher(int id)
        {
            var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(id);

            if (teacher == null)
            {
                logger.LogWarning("Teacher not found");
                return NotFound();
            }

            var teacherDto = this.mapper.Map<TeacherDto>(teacher);

            logger.LogInformation("Get teacher by id");
            return Ok(teacherDto);
        }

        // PUT: api/teacher/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, TeacherUpdateDto teacher)
        {
            var teacherDto = this.mapper.Map<Teacher>(teacher);
            if (id != teacherDto.Id)
            {
                logger.LogWarning("Id not match");
                return BadRequest();
            }

            await unitOfWork.TeacherRepository.UpdateAsync(teacherDto);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Update teacher");
            return NoContent();
        }

        // POST: api/teacher
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeacherCreateDto>> PostTeacher(TeacherCreateDto teacher)
        {
            var validationResult = this.validator.Validate(teacher);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation error");
                return BadRequest(validationResult.Errors);
            }

            var teacherDto = this.mapper.Map<Teacher>(teacher);
            await unitOfWork.TeacherRepository.AddAsync(teacherDto);
            await unitOfWork.SaveAsync();
            cache.Remove(TeachersCacheKey);

            logger.LogInformation("Create teacher");
            return CreatedAtAction("GetTeacher", new { id = teacherDto.Id }, teacherDto);
        }

        // DELETE: api/teacher/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            try
            {
                await unitOfWork.TeacherRepository.DeleteAsync(id);
                await unitOfWork.SaveAsync();
                cache.Remove(TeachersCacheKey);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogError(ex.Message);
                return NotFound();
            }

            logger.LogInformation("Delete teacher");
            return NoContent();
        }
    }
}
