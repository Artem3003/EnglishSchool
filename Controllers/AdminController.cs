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

namespace demo_english_school.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        // GET: api/Admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            return Ok(await adminService.GetAllAsync());
        }

        // GET: api/Admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            var admin = await adminService.GetByIdAsync(id);

            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        // PUT: api/Admin/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, Admin admin)
        {
            if (id != admin.Id)
            {
                return BadRequest();
            }

            await adminService.UpdateAsync(admin);

            return NoContent();
        }

        // POST: api/Admin
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
            await adminService.AddAsync(admin);

            

            return CreatedAtAction("GetAdmin", new { id = admin.Id }, admin);
        }

        // DELETE: api/Admin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            await adminService.DeleteAsync(id);

            return NoContent();
        }
    }
}
