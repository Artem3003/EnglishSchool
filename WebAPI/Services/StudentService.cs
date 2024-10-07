using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Data;
using demo_english_school.Interfaces;
using demo_english_school.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_english_school.Services;

public class StudentService : IStudentService
{
    private readonly DemoEnglishSchoolContext context;
    public StudentService()
    {
        this.context = new DemoEnglishSchoolContext();
    }
    public StudentService(DemoEnglishSchoolContext context)
    {
        this.context = context;
    }
    public async Task AddAsync(Student model)
    {
        ArgumentNullException.ThrowIfNull(model);

        await context.Students.AddAsync(model);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);

        var studentToRemove = await context.Students.FindAsync(modelId);
        if (studentToRemove != null)
        {
            context.Students.Remove(studentToRemove);
            await context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentNullException("Student not found");
        }
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await context.Students.ToListAsync();
    }

    public async Task<Student> GetByIdAsync(int id)
    {
        var student = await context.Students.FindAsync(id);
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

            context.Students.Update(studentToUpdate);
            await context.SaveChangesAsync();
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