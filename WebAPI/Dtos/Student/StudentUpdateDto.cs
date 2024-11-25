using demo_english_school.Models;

namespace demo_english_school.Dtos;

public class StudentUpdateDto
{
    public DateTime DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public UserUpdateDto? User { get; set; }
}