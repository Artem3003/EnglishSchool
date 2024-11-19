using demo_english_school.Interfaces;
using demo_english_school.Repositories;
using Microsoft.EntityFrameworkCore;
using WebAPI.Interfaces;

namespace demo_english_school.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private bool disposed;
    private IUserRepository? userRepository;
    private ITeacherRepository? teacherRepository;
    private IStudentRepository? studentRepository;
    private IAdminRepository? adminRepository;
    private readonly DemoEnglishSchoolContext context;

    public UnitOfWork()
    {
        this.context = new DemoEnglishSchoolContext(new DbContextOptions<DemoEnglishSchoolContext>());
    }

    public UnitOfWork(DemoEnglishSchoolContext context)
    {
        this.context = context;
    }

    public IUserRepository UserRepository => this.userRepository ??= new UserRepository(this.context);

    public ITeacherRepository TeacherRepository => this.teacherRepository ??= new TeacherRepository(this.context);

    public IStudentRepository StudentRepository => this.studentRepository ??= new StudentRepository(this.context);

    public IAdminRepository AdminRepository => this.adminRepository ??= new AdminRepository(this.context);

    public Task SaveAsync()
    {
        try
        {
            return this.context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Concurrency error", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Database update error", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error", ex);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed && disposing)
        {
            this.context.Dispose();
        }

        this.disposed = true;
    }
}