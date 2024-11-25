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

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IValidator<UserCreateDto> validator;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UserCreateDto> validator)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.validator = validator;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await unitOfWork.UserRepository.GetAllAsync();
            var usersDto = this.mapper.Map<IEnumerable<UserDto>>(users);

            return Ok(usersDto);
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = this.mapper.Map<UserDto>(user);

            return userDto;
        }

        // PUT: api/user/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            await unitOfWork.UserRepository.UpdateAsync(user);
            await unitOfWork.SaveAsync();

            return NoContent();
        }

        // POST: api/user
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserCreateDto>> PostUser(UserCreateDto user)
        {
            var result = await validator.ValidateAsync(user);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var userDto = this.mapper.Map<User>(user);
            await unitOfWork.UserRepository.AddAsync(userDto);
            await unitOfWork.SaveAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, userDto);
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await unitOfWork.UserRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
