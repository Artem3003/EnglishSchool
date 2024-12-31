using demo_english_school.Models;

namespace demo_english_school.Dtos;

public class TeacherUpdateDto
{
    public int Id { get; set; }
    public string? Bio { get; set; }
    
    public string? Qualification { get; set; }

    public int YearsOfExperience { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public UserUpdateDto? User { get; set; }
}