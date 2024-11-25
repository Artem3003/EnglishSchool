using System.Linq;
using AutoMapper;
using demo_english_school.Models;
using demo_english_school.Dtos;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace demo_english_school.Automapper;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Teacher, TeacherDto>()
            .ForMember(t => t.UserId, opt => opt.MapFrom(t => t.UserId))
            .ForMember(t => t.FullName, opt => opt.MapFrom(t => t.User!.FullName))
            .ForMember(t => t.Email, opt => opt.MapFrom(t => t.User!.Email))
            .ReverseMap();
        CreateMap<Teacher, TeacherCreateDto>()
            .ReverseMap();
        CreateMap<Teacher, TeacherUpdateDto>()
            .ReverseMap();

        CreateMap<Student, StudentDto>()
            .ForMember(s => s.UserId, opt => opt.MapFrom(s => s.UserId))
            .ForMember(s => s.FullName, opt => opt.MapFrom(s => s.User!.FullName))
            .ForMember(s => s.Email, opt => opt.MapFrom(s => s.User!.Email))
            .ReverseMap();
        CreateMap<Student, StudentCreateDto>()
            .ReverseMap();
        CreateMap<Student, StudentUpdateDto>()
            .ReverseMap();

        CreateMap<User, UserDto>()
            .ReverseMap();
        CreateMap<User, UserCreateDto>()
            .ReverseMap();
        CreateMap<User, UserUpdateDto>()
            .ReverseMap();

        CreateMap<Admin, AdminDto>()
            .ForMember(a => a.UserId, opt => opt.MapFrom(a => a.UserId))
            .ForMember(a => a.FullName, opt => opt.MapFrom(a => a.User!.FullName))
            .ReverseMap();
        CreateMap<Admin, AdminCreateDto>()
            .ReverseMap();
        CreateMap<Admin, AdminUpdateDto>()
            .ReverseMap();
    }
}