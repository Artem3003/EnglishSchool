using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Data;
using demo_english_school.Interfaces;
using demo_english_school.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_english_school.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly DemoEnglishSchoolContext context;
    public StudentRepository()
    {
        this.context = new DemoEnglishSchoolContext();
    }
    public StudentRepository(DemoEnglishSchoolContext context)
    {
        this.context = context;
    }
    public async Task AddAsync(Student model)
    {
        ArgumentNullException.ThrowIfNull(model);

        await context.Students.AddAsync(model);
    }

    public async Task DeleteAsync(int modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);

        var studentToRemove = await context.Students
            .Where(a => a.Id == modelId)
            .Include(a => a.User)
            .FirstOrDefaultAsync();

        if (studentToRemove == null)
        {
            throw new ArgumentNullException("Student not found");
        }

        context.Students.Remove(studentToRemove);
        if (studentToRemove.User != null)
        {
            context.Users.Remove(studentToRemove.User);
        }
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await context.Students.Include(s => s.User).ToListAsync();
    }

    public async Task<Student> GetByIdAsync(int id)
    {
        var student = await context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
        if (student != null)
        {
            return student;
        }
        else
        {
            throw new ArgumentNullException("Student not found");
        }
    }

    public async Task UpdateAsync(Student model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var studentToUpdate = await context.Students.FindAsync(model.Id);
        if (studentToUpdate != null)
        {
            studentToUpdate.Address = model.Address;
            studentToUpdate.DateOfBirth = model.DateOfBirth;
            studentToUpdate.Phone = model.Phone;
            studentToUpdate.UserId = model.UserId;

            if (studentToUpdate.User != null && model.User != null)
            {
                studentToUpdate.User.Username = model.User.Username;
                studentToUpdate.User.Password = model.User.Password;
                studentToUpdate.User.Email = model.User.Email;
                studentToUpdate.User.FullName = model.User.FullName;

                context.Users.Update(studentToUpdate.User);
            }

            context.Students.Update(studentToUpdate);
        }
        else
        {
            throw new ArgumentNullException("Student not found");
        }
    }

    public async Task<bool> StudentExistsAsync(int id)
    {
        return await context.Students.AnyAsync(a => a.Id == id);
    }
}