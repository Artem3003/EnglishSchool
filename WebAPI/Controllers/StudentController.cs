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

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public StudentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return this.Ok(await unitOfWork.StudentRepository.GetAllAsync());
        }

        // GET: api/student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await unitOfWork.StudentRepository.GetByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/student/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            await unitOfWork.StudentRepository.UpdateAsync(student);

            return NoContent();
        }

        // POST: api/student
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            await unitOfWork.StudentRepository.AddAsync(student);
            return CreatedAtAction("GetStudent", new { id = student.Id }, student);
        }

        // DELETE: api/student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await unitOfWork.StudentRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
