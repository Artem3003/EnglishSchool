using Bogus;
using demo_english_school.Models;

namespace demo_english_school.Data;

public static class SeedData
{
    public static void Seed(IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        {
            DemoEnglishSchoolContext context = scope.ServiceProvider.GetService<DemoEnglishSchoolContext>()
                ?? throw new InvalidOperationException("DemoEnglishSchoolContext service not found.");

            context.Database.EnsureCreated();

            // Users
            if (!context.Users.Any())
            {
                var userFaker = new Faker<User>()
                        .RuleFor(u => u.Username, f => f.Internet.UserName())
                        .RuleFor(u => u.Password, f => f.Internet.Password())
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.FullName, f => f.Name.FullName());

                var users = userFaker.Generate(20);
                context.Users.AddRange(users);
                context.SaveChanges();

                var userIds = users.Select(u => u.Id).ToList();
                var adminUserIds = userIds.Take(2).ToList();
                var teacherUserIds = userIds.Skip(2).Take(3).ToList();
                var studentUserIds = userIds.Skip(5).Take(5).ToList();

                // Admins
                if (!context.Admins.Any())
                {
                    var adminFaker = new Faker<Admin>()
                        .RuleFor(a => a.Role, f => f.PickRandom("Admin", "SuperAdmin", "Developer", "Manager"))
                        .RuleFor(a => a.UserId, f => f.PickRandom(adminUserIds));

                    context.Admins.AddRange(adminFaker.Generate(2));
                }

                // Teachers
                if (!context.Teachers.Any())
                {
                    var teacherFaker = new Faker<Teacher>()
                        .RuleFor(t => t.Bio, f => f.Lorem.Paragraph())
                        .RuleFor(t => t.Qualification, f => f.Lorem.Sentence())
                        .RuleFor(t => t.YearsOfExperience, f => f.Random.Number(1, 10))
                        .RuleFor(t => t.Phone, f => f.Phone.PhoneNumber())
                        .RuleFor(t => t.Address, f => f.Address.FullAddress())
                        .RuleFor(t => t.UserId, f => f.PickRandom(teacherUserIds));

                    context.Teachers.AddRange(teacherFaker.Generate(3));
                }

                // Students
                if (!context.Students.Any())
                {
                    var studentFaker = new Faker<Student>()
                        .RuleFor(s => s.DateOfBirth, f => f.Date.Past())
                        .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber())
                        .RuleFor(s => s.Address, f => f.Address.FullAddress())
                        .RuleFor(s => s.UserId, f => f.PickRandom(studentUserIds));

                    context.Students.AddRange(studentFaker.Generate(5));
                }

                context.SaveChanges();
            }
        }
    }
}