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
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using demo_english_school.Options;
using Microsoft.Extensions.Options;

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private const string AdminsCacheKey = "AdminsList";
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IValidator<AdminCreateDto> validator;
        private readonly ILogger<AdminController> logger;
        private readonly IMemoryCache cache;
        private readonly CacheSettings cacheSettings;
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public AdminController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AdminCreateDto> validator, ILogger<AdminController> logger, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.validator = validator;
            this.logger = logger;
            this.cache = cache;
            this.cacheSettings = cacheSettings.Value;
        }

        // GET: api/admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAdmins()
        {
            if (cache.TryGetValue(AdminsCacheKey, out IEnumerable<AdminDto>? adminsDto))
            {
                logger.LogInformation("Admins found in cache.");   
            }
            else
            {   
                try
                {
                    await semaphoreSlim.WaitAsync();
                    if (cache.TryGetValue(AdminsCacheKey, out adminsDto))
                    {
                        logger.LogInformation("Admins found in cache.");   
                    } 
                    else
                    {
                        logger.LogInformation("Admins not found in cache. Fetching from database.");
                        var admins = await unitOfWork.AdminRepository.GetAllAsync();
                        adminsDto = this.mapper.Map<IEnumerable<AdminDto>>(admins);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(cacheSettings.CacheDuration))
                            .SetAbsoluteExpiration(TimeSpan.FromHours(cacheSettings.CacheDuration))
                            .SetPriority(CacheItemPriority.Normal)
                            .SetSize(1);

                        cache.Set(AdminsCacheKey, adminsDto, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            return Ok(adminsDto);
        }

        // GET: api/admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminDto>> GetAdmin(int id)
        {
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                logger.LogWarning("Admin not found");
                return NotFound();
            }

            var adminDto = this.mapper.Map<AdminDto>(admin);

            logger.LogInformation("Get admin by id");
            return Ok(adminDto);
        }

        // PUT: api/admin/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, AdminUpdateDto admin)
        {
            var adminDto = this.mapper.Map<Admin>(admin);
            if (id != adminDto.Id)
            {
                logger.LogWarning("Id not match");
                return BadRequest();
            }

            await unitOfWork.AdminRepository.UpdateAsync(adminDto);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Update admin");
            return NoContent();
        }

        // POST: api/admin
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AdminCreateDto>> PostAdmin(AdminCreateDto admin)
        {
            var validationResult = this.validator.Validate(admin);  
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation error");
                return BadRequest(validationResult.Errors);
            }

            var adminDto = this.mapper.Map<Admin>(admin);
            await unitOfWork.AdminRepository.AddAsync(adminDto);
            await unitOfWork.SaveAsync();
            cache.Remove(AdminsCacheKey);

            logger.LogInformation("Create admin");
            return CreatedAtAction("GetAdmin", new { id = adminDto.Id }, adminDto);
        }

        // DELETE: api/admin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {
                await unitOfWork.AdminRepository.DeleteAsync(id);
                await unitOfWork.SaveAsync();
                cache.Remove(AdminsCacheKey);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogWarning(ex.Message);
                return NotFound();
            }

            logger.LogInformation("Delete admin");
            return NoContent();
        }
    }
}
