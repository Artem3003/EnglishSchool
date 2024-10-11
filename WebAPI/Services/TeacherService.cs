using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Data;
using demo_english_school.Interfaces;
using demo_english_school.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_english_school.Services;

public class TeacherService : ITeacherService
{
    private readonly DemoEnglishSchoolContext context;
    public TeacherService()
    {
        this.context = new DemoEnglishSchoolContext();
    }
    public TeacherService(DemoEnglishSchoolContext context)
    {
        this.context = context;
    }
    public async Task AddAsync(Teacher model)
    {
        ArgumentNullException.ThrowIfNull(model);

        await context.Teachers.AddAsync(model);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);

        var teacherToRemove = await context.Teachers
            .Where(a => a.Id == modelId)
            .Include(a => a.User)
            .FirstOrDefaultAsync();

        if (teacherToRemove == null)
        {
            throw new ArgumentNullException("Teacher not found");
        }

        context.Teachers.Remove(teacherToRemove);
        if (teacherToRemove.User != null)
        {
            context.Users.Remove(teacherToRemove.User);
        }
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Teacher>> GetAllAsync()
    {
        return await context.Teachers.ToListAsync();
    }

    public async Task<Teacher> GetByIdAsync(int id)
    {
        var teacher = await context.Teachers.FindAsync(id);
        if (teacher != null)
        {
            return teacher;
        }
        else
        {
            throw new ArgumentNullException("Teacher not found");
        }
    }

    public async Task UpdateAsync(Teacher model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var teacherToUpdate = await context.Teachers.FindAsync(model.Id);
        if (teacherToUpdate != null)
        {
            teacherToUpdate.Bio = model.Bio;
            teacherToUpdate.Qualification = model.Qualification;
            teacherToUpdate.YearsOfExperience = model.YearsOfExperience;
            teacherToUpdate.Phone = model.Phone;
            teacherToUpdate.Address = model.Address;
            teacherToUpdate.UserId = model.UserId;

            context.Teachers.Update(teacherToUpdate);
            await context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentNullException("Admin not found");
        }
    }

    public async Task<bool> TeacherExistsAsync(int id)
    {
        return await context.Teachers.AnyAsync(a => a.Id == id);
    }
}