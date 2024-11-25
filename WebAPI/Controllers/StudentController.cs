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

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IValidator<StudentCreateDto> validator;

        public StudentController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<StudentCreateDto> validator)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.validator = validator;
        }

        // GET: api/student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await unitOfWork.StudentRepository.GetAllAsync();
            var studentsDto = this.mapper.Map<IEnumerable<StudentDto>>(students);

            return Ok(studentsDto);
        }

        // GET: api/student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await unitOfWork.StudentRepository.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var studentDto = this.mapper.Map<StudentDto>(student);

            return studentDto;
        }

        // PUT: api/student/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, StudentUpdateDto student)
        {
            var studentDto = this.mapper.Map<Student>(student);
            if (id != studentDto.Id)
            {
                return BadRequest();
            }

            await unitOfWork.StudentRepository.UpdateAsync(studentDto);
            await unitOfWork.SaveAsync();

            return NoContent();
        }

        // POST: api/student
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentCreateDto>> PostStudent(StudentCreateDto student)
        {
            var result = await validator.ValidateAsync(student);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var studentDto = this.mapper.Map<Student>(student);
            await unitOfWork.StudentRepository.AddAsync(studentDto);
            await unitOfWork.SaveAsync();
            return CreatedAtAction("GetStudent", new { id = studentDto.Id }, studentDto);
        }

        // DELETE: api/student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await unitOfWork.StudentRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
