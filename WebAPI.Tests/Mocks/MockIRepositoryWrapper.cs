using demo_english_school.Interfaces;
using demo_english_school.Models;
using Moq;
using WebAPI.Interfaces;

namespace WebAPI.Mocks.Tests;

internal class MockRepositoryWrapper
{
    public static Mock<IUnitOfWork> GetMock()
    {
        var mock = new Mock<IUnitOfWork>();

        mock.Setup(u => u.UserRepository).Returns(() => GetUsers());
        mock.Setup(a => a.AdminRepository).Returns(() => GetAdmins());
        mock.Setup(t => t.TeacherRepository).Returns(() => GetTeachers());
        mock.Setup(s => s.StudentRepository).Returns(() => GetStudents());
        mock.Setup(m => m.SaveAsync()).Callback(() => { });

        return mock;
    }

    public static IEnumerable<Admin> GetAdmins()
    {
        return new List<Admin>
        {
            new Admin { Id = 1, Role = "Admin", UserId = 1, User = new User { Id = 1, Username = "Bob", Password = "Bob123", Email = "Bob@gmail.com", FullName = "Bob Smith"}},
            new Admin { Id = 2, Role = "Manadger", UserId = 2, User = new User { Id = 2, Username = "Max", Password = "Max123", Email = "Max@gmail.com", FullName = "Max Smith"} },
        };
    }

    public static IEnumerable<Student> GetStudents()
    {
        return new List<Student>
        {
            new Student { Id = 1, DateOfBirth = new DateTime(1999, 5, 23, 0, 0, 0, DateTimeKind.Utc), Phone="111-222-333-4444", Address="1234 Main St", UserId = 3 },
            new Student { Id = 2, DateOfBirth = new DateTime(2005, 2, 5, 0, 0, 0, DateTimeKind.Utc), Phone="222-333-444-5555", Address="5678 Main St", UserId = 4 },
        };
    }

    public static IEnumerable<Teacher> GetTeachers()
    {
        return new List<Teacher>
        {
            new Teacher { Id = 1, Bio = "Specializes in English literature and language.", Qualification = "MA in English Literature", YearsOfExperience = 10, Phone = "666-555-444-3333", Address = "568 Main St", UserId = 5 },
            new Teacher { Id = 2, Bio = "Specializes in English literature and language.", Qualification = "MA in English Literature", YearsOfExperience = 5, Phone = "777-888-999-0000", Address = "65 Main St", UserId = 6 },
        };
    }

    public static IEnumerable<User> GetUsers()
    {
        return new List<User>
        {
            new User { Id = 1, Username = "Bob", Password = "Bob123", Email = "Bob@gmail.com", FullName = "Bob Smith"},
            new User { Id = 2, Username = "Max", Password = "Max123", Email = "Max@gmail.com", FullName = "Max Smith"},
            new User { Id = 3, Username = "Will", Password = "Will123", Email = "Will@gmail.com", FullName = "Will Smith"},
            new User { Id = 4, Username = "Alex", Password = "Alex123", Email = "Alex@gmail.com", FullName = "Alex Smith"},
            new User { Id = 5, Username = "Bill", Password = "Bill123", Email = "Bill@gmail.com", FullName = "Bill Smith"},
            new User { Id = 6, Username = "Andrew", Password = "Andrew123", Email = "Andrew@gmail.com", FullName = "Andrew Smith"},
        };
    }
}