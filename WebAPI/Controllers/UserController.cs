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
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using demo_english_school.Options;
using Microsoft.Extensions.Options;

namespace demo_english_school.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private const string UsersCacheKey = "UsersList";
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IValidator<UserCreateDto> validator;
    private readonly ILogger<UserController> logger;
    private readonly IMemoryCache cache;
    private readonly CacheSettings cacheSettings;
    private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    public UserController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UserCreateDto> validator, ILogger<UserController> logger, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.validator = validator;
        this.logger = logger;
        this.cache = cache;
        this.cacheSettings = cacheSettings.Value;
        this.cacheSettings = cacheSettings.Value;
    }

    // GET: api/user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        if (cache.TryGetValue(UsersCacheKey, out IEnumerable<UserDto>? usersDto))
        {
            logger.LogInformation("Users found in cache.");
            return Ok(usersDto);
        }
        else
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                if (cache.TryGetValue(UsersCacheKey, out usersDto))
                {
                    logger.LogInformation("Users found in cache.");
                }
                else
                {
                    logger.LogInformation("Users not found in cache. Fetching from database.");
                    var users = await unitOfWork.UserRepository.GetAllAsync();
                    usersDto = this.mapper.Map<IEnumerable<UserDto>>(users);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(cacheSettings.CacheDuration))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(cacheSettings.CacheDuration))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1);

                    cache.Set(UsersCacheKey, usersDto, cacheEntryOptions);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        return Ok(usersDto);
    }

    // GET: api/user/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(id);

        if (user == null)
        {
            logger.LogWarning("User not found");
            return NotFound();
        }

        var userDto = this.mapper.Map<UserDto>(user);

        logger.LogInformation("Get user by id");
        return Ok(userDto);
    }

    // PUT: api/user/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, UserUpdateDto user)
    {
        var userDto = this.mapper.Map<User>(user);
        var existingUser = this.unitOfWork.UserRepository.GetAllAsync().Result.FirstOrDefault(u => u.Id == id);
        if (existingUser == null || id != existingUser.Id)
        {
            logger.LogWarning("Id not match");
            return BadRequest();
        }

        userDto.Id = id;
        await unitOfWork.UserRepository.UpdateAsync(userDto);
        await unitOfWork.SaveAsync();

        logger.LogInformation("Update user");
        return NoContent();
    }

    // POST: api/user
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<UserCreateDto>> PostUser(UserCreateDto user)
    {
        var validationResult = this.validator.Validate(user);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation error");
            return BadRequest(validationResult.Errors);
        }

        var userDto = this.mapper.Map<User>(user);
        await unitOfWork.UserRepository.AddAsync(userDto);
        await unitOfWork.SaveAsync();
        cache.Remove(UsersCacheKey);

        logger.LogInformation("Create user");
        return CreatedAtAction("GetUser", new { id = user.Id }, userDto);
    }

    // DELETE: api/user/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await unitOfWork.UserRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();
            cache.Remove(UsersCacheKey);
        }
        catch (ArgumentNullException ex)
        {
            logger.LogWarning(ex.Message);
            return NotFound();
        }

        logger.LogInformation("Delete user");
        return NoContent();
    }
}
