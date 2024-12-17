using demo_english_school.Data;
using demo_english_school.Validator;
using demo_english_school.Interfaces;
using demo_english_school.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using WebAPI.Interfaces;
using demo_english_school.Automapper;
using demo_english_school.Options;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddOptions<SwaggerSettings>().BindConfiguration("SwaggerSettings");
builder.Services.AddOptions<CacheSettings>().BindConfiguration("CacheSettings");

builder.Services.AddLogging(config => 
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddDbContext<DemoEnglishSchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DemoEnglishSchoolDb")));

builder.Services.AddAutoMapper(typeof(AutomapperProfile));

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<UserValidator>()
    .AddValidatorsFromAssemblyContaining<TeacherValidator>()
    .AddValidatorsFromAssemblyContaining<StudentValidator>()
    .AddValidatorsFromAssemblyContaining<AdminValidator>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllers();

builder.Services.AddMemoryCache(opt => 
{
    opt.SizeLimit = 1024;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        var swaggerSettings = builder.Configuration.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();
        options.SwaggerDoc(swaggerSettings?.Version, new()
        {
            Title = swaggerSettings?.Title,
            Version = swaggerSettings?.Version,
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint($"/swagger/{builder.Configuration["SwaggerSettings:Version"]}/swagger.json", builder.Configuration["SwaggerSettings:Title"]);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

SeedData.Seed(app);

await app.RunAsync();