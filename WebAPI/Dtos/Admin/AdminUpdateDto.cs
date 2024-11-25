using demo_english_school.Models;

namespace demo_english_school.Dtos;

public class AdminUpdateDto
{
    public int Id { get; set; }

    public string? Role { get; set; }

    public UserUpdateDto? User { get; set; }
}