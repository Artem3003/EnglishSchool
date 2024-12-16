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

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IValidator<AdminCreateDto> validator;
        private readonly ILogger<AdminController> logger;

        public AdminController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AdminCreateDto> validator, ILogger<AdminController> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.validator = validator;
            this.logger = logger;
        }

        // GET: api/admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAdmins()
        {
            var admins = await unitOfWork.AdminRepository.GetAllAsync();
            var adminsDto = this.mapper.Map<IEnumerable<AdminDto>>(admins);

            logger.LogInformation("Get all admins");
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

            logger.LogInformation("Create admin");
            return CreatedAtAction("GetAdmin", new { id = adminDto.Id }, adminDto);
        }

        // DELETE: api/admin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            await unitOfWork.AdminRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Delete admin");
            return NoContent();
        }
    }
}
