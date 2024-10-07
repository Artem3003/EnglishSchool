using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Data;
using demo_english_school.Interfaces;
using demo_english_school.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_english_school.Services;

public class AdminService : IAdminService
{
    private readonly DemoEnglishSchoolContext context;
    public AdminService()
    {
        this.context = new DemoEnglishSchoolContext();
    }
    public AdminService(DemoEnglishSchoolContext context)
    {
        this.context = context;
    }
    public async Task AddAsync(Admin model)
    {
        ArgumentNullException.ThrowIfNull(model);

        await context.Admins.AddAsync(model);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);

        var adminToRemove = await context.Admins.FindAsync(modelId);
        if (adminToRemove != null)
        {
            context.Admins.Remove(adminToRemove);
            await context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentNullException("Admin not found");
        }
    }

    public async Task<IEnumerable<Admin>> GetAllAsync()
    {
        return await context.Admins.ToListAsync();
    }

    public async Task<Admin> GetByIdAsync(int id)
    {
        var admin = await context.Admins.FindAsync(id);
        if (admin != null)
        {
            return admin;
        }
        else
        {
            throw new ArgumentNullException("Admin not found");
        }
    }

    public async Task UpdateAsync(Admin model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var adminToUpdate = await context.Admins.FindAsync(model.Id);
        if (adminToUpdate != null)
        {
            adminToUpdate.Role = model.Role;
            adminToUpdate.UserId = model.UserId;

            context.Admins.Update(adminToUpdate);
            await context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentNullException("Admin not found");
        }
    }

    public async Task<bool> AdminExistsAsync(int id)
    {
        return await context.Admins.AnyAsync(a => a.Id == id);
    }
}