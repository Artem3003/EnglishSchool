using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Data;
using demo_english_school.Interfaces;
using demo_english_school.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_english_school.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DemoEnglishSchoolContext context;
    public UserRepository()
    {
        this.context = new DemoEnglishSchoolContext();
    }
    public UserRepository(DemoEnglishSchoolContext context)
    {
        this.context = context;
    }
    public async Task AddAsync(User model)
    {
        ArgumentNullException.ThrowIfNull(model);

        await context.Users.AddAsync(model);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);

        var userToRemove = await context.Users
            .Where(a => a.Id == modelId)
            .FirstOrDefaultAsync();

        if (userToRemove == null)
        {
            throw new ArgumentNullException("User not found");
        }
        
        context.Users.Remove(userToRemove);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User> GetByIdAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            return user;
        }
        else
        {
            throw new ArgumentNullException("User not found");
        }
    }

    public async Task UpdateAsync(User model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var userToUpdate = await context.Users.FindAsync(model.Id);
        if (userToUpdate != null)
        {
            userToUpdate.Username = model.Username;
            userToUpdate.Password = model.Password;
            userToUpdate.Email = model.Email;
            userToUpdate.FullName = model.FullName;
            context.Users.Update(userToUpdate);
            await context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentNullException("User not found");
        }
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        return await context.Users.AnyAsync(a => a.Id == id);
    }
}