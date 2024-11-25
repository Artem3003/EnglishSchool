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

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public TeacherController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        // GET: api/teacher
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachers()
        {
            var teachers = await unitOfWork.TeacherRepository.GetAllAsync();
            var teachersDto = this.mapper.Map<IEnumerable<TeacherDto>>(teachers);

            return Ok(teachersDto);
        }

        // GET: api/teacher/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacher(int id)
        {
            var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            var teacherDto = this.mapper.Map<TeacherDto>(teacher);

            return teacherDto;
        }

        // PUT: api/teacher/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, TeacherUpdateDto teacher)
        {
            var teacherDto = this.mapper.Map<Teacher>(teacher);
            if (id != teacherDto.Id)
            {
                return BadRequest();
            }

            await unitOfWork.TeacherRepository.UpdateAsync(teacherDto);
            await unitOfWork.SaveAsync();

            return NoContent();
        }

        // POST: api/teacher
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeacherCreateDto>> PostTeacher(TeacherCreateDto teacher)
        {
            var teacherDto = this.mapper.Map<Teacher>(teacher);
            await unitOfWork.TeacherRepository.AddAsync(teacherDto);
            await unitOfWork.SaveAsync();

            return CreatedAtAction("GetTeacher", new { id = teacherDto.Id }, teacherDto);
        }

        // DELETE: api/teacher/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            await unitOfWork.TeacherRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
