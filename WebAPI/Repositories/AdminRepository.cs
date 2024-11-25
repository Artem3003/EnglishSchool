using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using demo_english_school.Data;
using demo_english_school.Interfaces;
using demo_english_school.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_english_school.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly DemoEnglishSchoolContext context;

    public AdminRepository()
    {
        this.context = new DemoEnglishSchoolContext();
    }
    public AdminRepository(DemoEnglishSchoolContext context)
    {
        this.context = context;
    }
    public async Task AddAsync(Admin model)
    {
        ArgumentNullException.ThrowIfNull(model);

        await context.Admins.AddAsync(model);
    }

    public async Task DeleteAsync(int modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);

        var adminToRemove = await context.Admins
            .Where(a => a.Id == modelId)
            .Include(a => a.User)
            .FirstOrDefaultAsync();

        if (adminToRemove == null)
        {
            throw new ArgumentNullException("Admin not found");
        }

        context.Admins.Remove(adminToRemove);
        if (adminToRemove.User != null)
        {
            context.Users.Remove(adminToRemove.User);
        }
    }

    public async Task<IEnumerable<Admin>> GetAllAsync()
    {
        return await context.Admins.Include(a => a.User).ToListAsync();
    }

    public async Task<Admin> GetByIdAsync(int id)
    {
        var admin = await context.Admins.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
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

        var adminToUpdate = await context.Admins.Include(u => u.User).FirstOrDefaultAsync(a => a.Id == model.Id);
        if (adminToUpdate != null)
        {
            adminToUpdate.Role = model.Role;
            adminToUpdate.UserId = model.UserId;
            adminToUpdate.User = model.User;

            if (adminToUpdate.User != null && model.User != null)
            {
                adminToUpdate.User.Username = model.User.Username;
                adminToUpdate.User.Password = model.User.Password;
                adminToUpdate.User.Email = model.User.Email;
                adminToUpdate.User.FullName = model.User.FullName;

                context.Users.Update(adminToUpdate.User);
            }

            context.Admins.Update(adminToUpdate);
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