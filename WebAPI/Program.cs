using demo_english_school.Data;
using demo_english_school.Validator;
using demo_english_school.Interfaces;
using demo_english_school.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container

builder.Services.AddDbContext<DemoEnglishSchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DemoEnglishSchoolDb")));

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<UserValidator>()
    .AddValidatorsFromAssemblyContaining<TeacherValidator>()
    .AddValidatorsFromAssemblyContaining<StudentValidator>()
    .AddValidatorsFromAssemblyContaining<AdminValidator>();

builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "English School API", Version = "v1" });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "English School API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

SeedData.Seed(app);

await app.RunAsync();